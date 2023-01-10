using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
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

namespace Irc.Client.Wpf.ViewModels
{
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
        [NotifyCanExecuteChangedFor(nameof(SendAsyncCommand))]
        private string textMessage;


        [ObservableProperty]
        private bool isTextMessageFocused;


        public IrcClient IrcClient { get; }
        private Task ircClientTask;
        private CancellationTokenSource cancellationTokenSource;

        //public Domain.Irc Irc { get; }

        private readonly IMessenger messenger;

        public IrcViewModel(IMessenger messenger)
        {
            State = ConnectionState.Disconnected;
            Host = "irc.irc-hispano.org";
            Nickname = "Nekketsu";

            Status = new();
            Chats = new() { Status };

            FocusInput();

            IrcClient = new(Nickname, Host);

            PropertyChanged += IrcViewModel_PropertyChanged;

            this.messenger = messenger;

            messenger.Register<QueryRequest>(this, (r, m) =>
            {
                Query(m.Nickname);
            });

            messenger.Register<WhoisRequest>(this, async (r, m) =>
            {
                await Whois(m.Nickname);
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

            IrcClient.Connected += IrcClient_Connected;
            IrcClient.MessageSent += IrcClient_MessageSent;
            IrcClient.MessageReceived += IrcClient_MessageReceived;
            IrcClient.RawMessageSent += IrcClient_RawMessageSent;
            IrcClient.RawMessageReceived += IrcClient_RawMessageReceived;

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
        private async void SendAsync()
        {
            var isProcessed = await ProcessClientCommand();

            if (isProcessed || State != ConnectionState.Connected)
            {
                return;
            }

            var message = TextMessage.StartsWith("/")
                ? Message.Parse(TextMessage.Substring(1))
                : SelectedTab is ChatViewModel selectedChat
                    ? new PrivMsgMessage(selectedChat?.Target, TextMessage)
                    : null;

            if (message is null)
            {
                var log = new MessageViewModel($"Error - Message not found: {TextMessage}");
                Status.Log.Add(log);
                return;
            }

            await IrcClient.SendMessageAsync(message);

            TextMessage = null;
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
        private async void CloseChat(object chat)
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

        private async Task<bool> ProcessClientCommand()
        {
            if (TextMessage.Equals("/server", StringComparison.InvariantCulture) || TextMessage.StartsWith("/server ", StringComparison.InvariantCultureIgnoreCase))
            {
                if (State == ConnectionState.Connected)
                {
                    Disconnect();
                }

                var messageSplit = TextMessage.Split();
                if (messageSplit.Length > 1)
                {
                    Host = messageSplit[1];
                }

                Connect();

                TextMessage = null;

                return true;
            }

            if (State != ConnectionState.Connected)
            {
                return false;
            }


            if (TextMessage.StartsWith("/query ", StringComparison.InvariantCultureIgnoreCase))
            {
                var messageSplit = TextMessage.Split();
                if (messageSplit.Length > 1)
                {
                    var target = messageSplit[1];
                    Query(target);
                }

                TextMessage = null;

                return true;
            }
            else if (TextMessage.Equals("/away", StringComparison.InvariantCultureIgnoreCase))
            {
                var message = new AwayMessage();
                await IrcClient.SendMessageAsync(message);

                TextMessage = null;

                return true;
            }
            else if (TextMessage.StartsWith("/away ", StringComparison.InvariantCultureIgnoreCase))
            {
                var text = TextMessage.Substring("/away ".Length).TrimStart();
                var awayMessage = new AwayMessage(text);
                await IrcClient.SendMessageAsync(awayMessage);

                TextMessage = null;

                return true;
            }
            else if (TextMessage.StartsWith("/notice ", StringComparison.InvariantCultureIgnoreCase))
            {
                var messageSplit = TextMessage.Split();

                var target = messageSplit[1];
                var text = TextMessage
                    .Substring(messageSplit[0].Length).TrimStart()
                    .Substring(messageSplit[1].Length).TrimStart();

                var message = new NoticeMessage(target, text);

                await IrcClient.SendMessageAsync(message);

                TextMessage = null;

                return true;
            }
            else if (TextMessage.StartsWith("/msg ", StringComparison.InvariantCultureIgnoreCase))
            {
                var messageSplit = TextMessage.Split();

                var target = messageSplit[1];
                var text = TextMessage
                    .Substring(messageSplit[0].Length).TrimStart()
                    .Substring(messageSplit[1].Length).TrimStart();

                var message = new PrivMsgMessage(target, text);

                await IrcClient.SendMessageAsync(message);

                TextMessage = null;

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
                nickname = nickname.Substring(1);
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
}
