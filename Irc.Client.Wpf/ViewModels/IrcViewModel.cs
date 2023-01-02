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
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Irc.Client.Wpf.ViewModels
{
    public partial class IrcViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ConnectAsyncCommand))]
        private ConnectionState state;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ConnectAsyncCommand))]
        private string host;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ConnectAsyncCommand))]
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


        private IrcClient ircClient;
        private CancellationTokenSource cancellationTokenSource;

        public Domain.Irc Irc { get; }

        private readonly IMessenger messenger;

        public IrcViewModel(IMessenger messenger)
        {
            State = ConnectionState.Disconnected;
            Host = "irc.irc-hispano.org";
            Nickname = "Nekketsu";

            Status = new();
            Chats = new() { Status };

            FocusInput();

            Irc = new();

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
                    if (SelectedTab is ChatViewModel selectedChat)
                    {
                        selectedChat.IsDirty = false;
                    }
                    FocusInput();
                    break;

                case nameof(SelectedTabIndex):
                    FocusInput();
                    break;
            }
        }


        [RelayCommand(CanExecute = nameof(CanConnect))]
        private async void ConnectAsync()
        {
            State = ConnectionState.Connecting;
            cancellationTokenSource = new CancellationTokenSource();

            ircClient = new IrcClient(Nickname, Host);
            ircClient.MessageSent += IrcClient_MessageSent;
            ircClient.MessageReceived += IrcClient_MessageReceived;
            ircClient.RawMessageSent += IrcClient_RawMessageSent;
            ircClient.RawMessageReceived += IrcClient_RawMessageReceived;

            await ircClient.RunAsync(cancellationTokenSource.Token);

            State = ConnectionState.Connected;

            FocusInput();

            Irc.Connect(Nickname);
        }

        private bool CanConnect() =>
            State == ConnectionState.Disconnected
                  && !string.IsNullOrEmpty(Host)
                  && !string.IsNullOrEmpty(Nickname);

        private void Disconnect()
        {
            ircClient.MessageReceived -= IrcClient_MessageReceived;
            ircClient.MessageSent -= IrcClient_MessageSent;
            ircClient.RawMessageSent += IrcClient_RawMessageSent;
            ircClient.RawMessageReceived += IrcClient_RawMessageReceived;
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

            await ircClient.SendMessageAsync(message);

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
                var partMessage = new PartMessage(channelViewModel.Target, null);
                await ircClient.SendMessageAsync(partMessage);

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

                var messageSplit = TextMessage.Split(' ');
                if (messageSplit.Length > 1)
                {
                    Host = messageSplit[1];
                }

                ConnectAsync();

                TextMessage = null;

                return true;
            }

            if (State != ConnectionState.Connected)
            {
                return false;
            }


            if (TextMessage.StartsWith("/query ", StringComparison.InvariantCultureIgnoreCase))
            {
                var messageSplit = TextMessage.Split(' ');
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
                var awayMessage = new AwayMessage();
                await ircClient.SendMessageAsync(awayMessage);

                TextMessage = null;

                return true;
            }
            else if (TextMessage.StartsWith("/away ", StringComparison.InvariantCultureIgnoreCase))
            {
                var text = TextMessage.Substring("/away ".Length).TrimStart();
                var awayMessage = new AwayMessage(text);
                await ircClient.SendMessageAsync(awayMessage);

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
            await ircClient.SendMessageAsync(message);
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

        private async void IrcClient_MessageSent(object sender, Message message)
        {
            await HandleMessageAsync(message);
        }

        private async void IrcClient_MessageReceived(object sender, Message message)
        {
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
    }
}
