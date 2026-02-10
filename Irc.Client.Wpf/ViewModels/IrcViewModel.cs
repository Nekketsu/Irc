using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Irc.Client.Scripting;
using Irc.Client.Wpf.MessageHandlers;
using Irc.Client.Wpf.Messenger.Requests;
using Irc.Client.Wpf.Model;
using Irc.Client.Wpf.ViewModels.Tabs;
using Irc.Client.Wpf.ViewModels.Tabs.Messages;
using Irc.Messages;
using Irc.Messages.Messages;
using Irc.Messages.Messages.OptionalFeatures;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

namespace Irc.Client.Wpf.ViewModels;

public partial class IrcViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConnectCommand))]
    private ConnectionState state;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConnectCommand))]
    private string host;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConnectCommand))]
    private string nickname;


    [ObservableProperty]
    private StatusViewModel status;

    [ObservableProperty]
    private ObservableCollection<ITabViewModel> chats;

    [ObservableProperty]
    private ITabViewModel selectedTab;

    [ObservableProperty]
    private int selectedTabIndex;

    private int previousTabIndex;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SendCommand))]
    private string textMessage;


    [ObservableProperty]
    private bool isTextMessageFocused;


    public IrcClient IrcClient { get; set; }
    private Task ircClientTask;
    private CancellationTokenSource cancellationTokenSource;

    private readonly IMessenger messenger;
    private readonly ScriptManager scriptManager;
    private readonly ScriptCommandHandler scriptCommandHandler;

    // Public property to allow MainWindow to access the same ScriptManager instance
    public ScriptManager ScriptManager => scriptManager;

    public IrcViewModel(IMessenger messenger)
    {
        State = ConnectionState.Disconnected;
        Host = "irc.irc-hispano.org";
        Nickname = "Nekketsu";

        Status = new();
        Chats = [Status];

        FocusInput();

        PropertyChanged += IrcViewModel_PropertyChanged;

        this.messenger = messenger;

        // Initialize ScriptManager without client (will be set when connected)
        // Pass log action to display script logs in Status tab
        this.scriptManager = new ScriptManager(null, null, LogScriptMessage);
        this.scriptCommandHandler = new ScriptCommandHandler(scriptManager);
        _ = scriptManager.LoadScriptsFromFolderAsync();

        messenger.Register<QueryRequest>(this, (r, m) =>
        {
            Query(m.Nickname);
        });

        messenger.Register<WhoisRequest>(this, async (r, m) =>
        {
            await Whois(m.Nickname);
        });

        messenger.Register<QuitRequest>(this, (r, m) =>
        {
            Disconnect();
        });
    }

    private void IrcViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(SelectedTab):
                if (SelectedTab is null)
                {
                    return;
                }
                SelectedTab.IsDirty = false;
                FocusInput();
                UpdateTitle();
                break;

            case nameof(SelectedTabIndex):
                FocusInput();
                break;
        }
    }


    [RelayCommand(CanExecute = nameof(CanConnect))]
    private void Connect()
    {
        State = ConnectionState.Connecting;
        cancellationTokenSource = new CancellationTokenSource();

        IrcClient = new(Nickname, Host);

        IrcClient.Connected += IrcClient_Connected;
        IrcClient.MessageSent += IrcClient_MessageSent;
        IrcClient.MessageReceived += IrcClient_MessageReceived;
        IrcClient.RawMessageSent += IrcClient_RawMessageSent;
        IrcClient.RawMessageReceived += IrcClient_RawMessageReceived;

        // Set the IrcClient in ScriptManager
        scriptManager.SetClient(IrcClient);

        ircClientTask = IrcClient.RunAsync(cancellationTokenSource.Token);
    }

    private bool CanConnect() =>
        State == ConnectionState.Disconnected
              && !string.IsNullOrEmpty(Host)
              && !string.IsNullOrEmpty(Nickname);

    private void Disconnect()
    {
        IrcClient.MessageReceived -= IrcClient_MessageReceived;
        IrcClient.MessageSent -= IrcClient_MessageSent;
        IrcClient.RawMessageSent += IrcClient_RawMessageSent;
        IrcClient.RawMessageReceived += IrcClient_RawMessageReceived;
        cancellationTokenSource.Cancel();

        State = ConnectionState.Disconnected;
    }

    [RelayCommand(CanExecute = nameof(CanSend))]
    private async Task Send()
    {
        var textMessage = TextMessage;
        TextMessage = null;

        var isProcessed = await ProcessClientCommand(textMessage);

        if (isProcessed || State != ConnectionState.Connected)
        {
            return;
        }

        var message = textMessage.StartsWith("/")
            ? Message.Parse(textMessage[1..])
            : SelectedTab is ChatViewModel selectedChat
                ? new PrivMsgMessage(selectedChat?.Target, textMessage)
                : null;

        if (message is null)
        {
            var log = new MessageViewModel($"Error - Message not found: {textMessage}");
            Status.Log.Add(log);
            return;
        }

        await IrcClient.SendMessageAsync(message);
    }

    private bool CanSend() => !string.IsNullOrEmpty(TextMessage);

    [RelayCommand]
    private void NextTab() => SelectedTabIndex = (SelectedTabIndex + 1) % Chats.Count;

    [RelayCommand]
    private void PreviousTab() => SelectedTabIndex = (SelectedTabIndex - 1 + Chats.Count) % Chats.Count;

    [RelayCommand(CanExecute = nameof(CanSelectTab))]
    private void SelectTab(string tabIndexString)
    {
        if (!int.TryParse(tabIndexString, out var tabIndex))
        {
            return;
        }

        var targetTabIndex = (tabIndex - 1 + Chats.Count) % Chats.Count;
        if (targetTabIndex == SelectedTabIndex)
        {
            targetTabIndex = previousTabIndex;
        }

        previousTabIndex = SelectedTabIndex;
        SelectedTabIndex = targetTabIndex % Chats.Count;
    }

    private bool CanSelectTab(string tabIndexString) => int.TryParse(tabIndexString, out var tabIndex) && tabIndex > 0 && tabIndex <= Chats.Count;

    [RelayCommand(CanExecute = nameof(CanCloseChat))]
    private async Task CloseChat(object chat)
    {
        if (chat is ChatViewModel chatViewModel)
        {
            Chats.Remove(chatViewModel);
        }
        if (chat is ChannelViewModel channelViewModel)
        {
            var message = new PartMessage(channelViewModel.Target, null);
            await IrcClient.SendMessageAsync(message);

            Chats.Remove(channelViewModel);
        }
    }

    private bool CanCloseChat(object chat) => chat is ChatViewModel or ChannelViewModel;

    private async Task<bool> ProcessClientCommand(string textMessage)
    {
        // Handle script commands
        if (textMessage.Equals("/script", StringComparison.InvariantCultureIgnoreCase))
        {
            // Open Script Manager dialog
            messenger.Send(new OpenScriptManagerDialogRequest());
            return true;
        }

        if (textMessage.StartsWith("/script ", StringComparison.InvariantCultureIgnoreCase))
        {
            var command = textMessage[8..].Trim(); // Remove "/script " prefix

            try
            {
                var result = await scriptCommandHandler.ExecuteAsync(command);

                var messageText = result.Success
                    ? $"✓ {result.Message}"
                    : $"✗ {result.Message}";

                var messageViewModel = new MessageViewModel(messageText);
                Status.Log.Add(messageViewModel);
            }
            catch (InvalidOperationException ex)
            {
                var messageViewModel = new MessageViewModel($"✗ {ex.Message}");
                Status.Log.Add(messageViewModel);
            }

            return true;
        }

        if (textMessage.Equals("/server", StringComparison.InvariantCulture) || textMessage.StartsWith("/server ", StringComparison.InvariantCultureIgnoreCase))
        {
            if (State == ConnectionState.Connected)
            {
                Disconnect();
            }

            var messageSplit = textMessage.Split();
            if (messageSplit.Length > 1)
            {
                Host = messageSplit[1];
            }

            Connect();

            return true;
        }

        if (State != ConnectionState.Connected)
        {
            return false;
        }


        if (textMessage.StartsWith("/query ", StringComparison.InvariantCultureIgnoreCase))
        {
            var messageSplit = textMessage.Split();
            if (messageSplit.Length > 1)
            {
                var target = messageSplit[1];
                Query(target);
            }

            return true;
        }
        else if (textMessage.Equals("/away", StringComparison.InvariantCultureIgnoreCase))
        {
            var message = new AwayMessage();
            await IrcClient.SendMessageAsync(message);

            return true;
        }
        else if (textMessage.StartsWith("/away ", StringComparison.InvariantCultureIgnoreCase))
        {
            var text = textMessage["/away ".Length..].TrimStart();
            var awayMessage = new AwayMessage(text);
            await IrcClient.SendMessageAsync(awayMessage);

            return true;
        }
        else if (textMessage.StartsWith("/notice ", StringComparison.InvariantCultureIgnoreCase))
        {
            var messageSplit = textMessage.Split();

            var target = messageSplit[1];
            var text = textMessage
[messageSplit[0].Length..].TrimStart()
[messageSplit[1].Length..].TrimStart();

            var message = new NoticeMessage(target, text);

            await IrcClient.SendMessageAsync(message);

            return true;
        }
        else if (textMessage.StartsWith("/msg ", StringComparison.InvariantCultureIgnoreCase))
        {
            var messageSplit = textMessage.Split();

            var target = messageSplit[1];
            var text = textMessage
[messageSplit[0].Length..].TrimStart()
[messageSplit[1].Length..].TrimStart();

            var message = new PrivMsgMessage(target, text);

            await IrcClient.SendMessageAsync(message);

            return true;
        }
        else
        {
            return false;
        }
    }

    private void Query(string nickname)
    {
        var chat = GetOrCreateChat(nickname);
        FocusChat(chat);
    }

    private async Task Whois(string nickname)
    {
        if (nickname.StartsWith('@'))
        {
            nickname = nickname[1..];
        }
        var message = new WhoisMessage(nickname);
        await IrcClient.SendMessageAsync(message);
    }

    public void FocusChat(ChatViewModel chat)
    {
        SelectedTab = chat;
    }

    private void FocusInput()
    {
        IsTextMessageFocused = false;
        IsTextMessageFocused = true;
    }

    private void IrcClient_Connected(object sender, EventArgs e)
    {
        State = ConnectionState.Connected;
        FocusInput();
    }

    private async void IrcClient_MessageSent(object sender, Message message)
    {
        await HandleMessageAsync(message);
    }

    private async void IrcClient_MessageReceived(object sender, Message message)
    {
        if (message is null)
        {
            return;
        }

        await HandleMessageAsync(message);
    }

    private void IrcClient_RawMessageSent(object sender, string message)
    {
        var now = TimeOnly.FromDateTime(DateTime.Now);
        Debug.WriteLine($">> [{now}] {message}");
    }

    private void IrcClient_RawMessageReceived(object sender, string message)
    {
        var now = TimeOnly.FromDateTime(DateTime.Now);
        Debug.WriteLine($"<< [{now}] {message}");
    }

    private async Task HandleMessageAsync(Message message)
    {
        if (await IMessageHandler.HandleAsync(message))
        {
            return;
        }
        else
        {
            var messageViewModel = new MessageViewModel(message.ToString());
            Status.Log.Add(messageViewModel);
        }
    }

    private void LogScriptMessage(string message)
    {
        // Log script messages to Status tab
        var messageViewModel = new MessageViewModel($"[SCRIPT] {message}");
        Status.Log.Add(messageViewModel);
    }

    public ChatViewModel DrawMessage(string target, MessageViewModel message)
    {
        var chat = GetOrCreateChat(target);

        return DrawMessage(chat, message);
    }

    public ChatViewModel DrawMessage(ChatViewModel chat, MessageViewModel message)
    {
        chat.Chat.Add(message);

        if (chat != SelectedTab)
        {
            chat.IsDirty = true;
        }

        return chat;
    }

    public void DrawMessage(ITabViewModel tab, MessageViewModel message)
    {
        if (tab is ChatViewModel chat)
        {
            DrawMessage(chat, message);
        }
        else if (tab is StatusViewModel status)
        {
            status.Log.Add(message);
            if (tab != SelectedTab)
            {
                tab.IsDirty = true;
            }
        }
    }

    public void DrawMessage(MessageViewModel message)
    {
        DrawMessage(SelectedTab, message);
    }

    private ChatViewModel GetOrCreateChat(string target)
    {
        var chat = Chats.OfType<ChatViewModel>().SingleOrDefault(chat => chat.Target is not null && chat.Target.Equals(target, StringComparison.InvariantCultureIgnoreCase));
        if (chat is null)
        {
            chat = target.StartsWith('#')
                ? new ChannelViewModel(target)
                : new ChatViewModel(target);

            Chats.Add(chat);
        }

        return chat;
    }

    public void UpdateTitle()
    {
        string title = null;

        switch (SelectedTab)
        {
            case StatusViewModel:
                title = "Status";
                break;
            case ChannelViewModel channel:
                title = channel.Target;
                var topic = IrcClient.Channels[channel.Target].Topic;
                if (topic is not null)
                {
                    title = $"{title}: {topic}";
                }
                break;
            case ChatViewModel chat:
                title = chat.Target;
                break;
        }

        messenger.Send(new TitleRequest(title));
    }
}
