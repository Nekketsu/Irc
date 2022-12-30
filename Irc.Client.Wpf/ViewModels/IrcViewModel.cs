using Irc.Client.Wpf.Model;
using Irc.Client.Wpf.ViewModels.Tabs;
using Irc.Messages;
using Irc.Messages.Messages;
using Messages.Replies.CommandResponses;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Documents;
using System.Windows.Input;

namespace Irc.Client.Wpf.ViewModels
{
    public class IrcViewModel : BindableBase
    {
        private ConnectionState state;
        private string host;
        private string nickname;

        private StatusViewModel status;
        private ObservableCollection<ChatViewModel> chats;
        private object selectedTab;
        private int selectedTabIndex;
        private string textMessage;

        private bool isTextMessageFocused;


        public ICommand ConnectCommand { get; set; }
        public ICommand SendCommand { get; set; }
        public ICommand NextTabCommand { get; set; }
        public ICommand PreviousTabCommand { get; set; }
        public ICommand SelectTabCommand { get; set; }
        public ICommand CloseChatCommand { get; set; }

        private IrcClient ircClient;
        private CancellationTokenSource cancellationTokenSource;

        private Model.Irc Irc { get; }

        public IrcViewModel()
        {
            State = ConnectionState.Disconnected;
            Host = "irc.irc-hispano.org";
            Nickname = "Nekketsu";

            Status = new StatusViewModel();
            Chats = new ObservableCollection<ChatViewModel>();

            IsTextMessageFocused = true;


            ConnectCommand = new DelegateCommand(ConnectAsync, () => State == ConnectionState.Disconnected && !string.IsNullOrEmpty(Host) && !string.IsNullOrEmpty(Nickname))
                .ObservesProperty(() => State)
                .ObservesProperty(() => Host)
                .ObservesProperty(() => Nickname);

            SendCommand = new DelegateCommand(SendAsync, () => !string.IsNullOrEmpty(TextMessage))
                .ObservesProperty(() => TextMessage);

            NextTabCommand = new DelegateCommand(NextTab);
            PreviousTabCommand = new DelegateCommand(PreviousTab);
            SelectTabCommand = new DelegateCommand<string>(SelectTab, CanSelectTab);
            CloseChatCommand = new DelegateCommand<ChatViewModel>(CloseChat, CanCloseChat);

            PropertyChanged += IrcViewModel_PropertyChanged;
            Irc = new Model.Irc();
        }

        private void IrcViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedTab))
            {
                if (SelectedTab is ChatViewModel selectedChat)
                {
                    selectedChat.IsDirty = false;
                }
            }
        }

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

            IsTextMessageFocused = true;

            Irc.Users.Add(Nickname, new Model.User(Nickname));
        }

        private void Disconnect()
        {
            ircClient.MessageReceived -= IrcClient_MessageReceived;
            ircClient.MessageSent -= IrcClient_MessageSent;
            ircClient.RawMessageSent += IrcClient_RawMessageSent;
            ircClient.RawMessageReceived += IrcClient_RawMessageReceived;
            cancellationTokenSource.Cancel();

            State = ConnectionState.Disconnected;
        }

        private async void SendAsync()
        {
            var isProcessed = ProcessClientCommand();

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
                Status.Log.Add($"Error - Message not found: {TextMessage}");
                return;
            }

            await ircClient.SendMessageAsync(message);

            TextMessage = null;
        }

        private void NextTab() => SelectedTabIndex = (SelectedTabIndex + 1) % (Chats.Count + 1);

        private void PreviousTab() => SelectedTabIndex = (SelectedTabIndex + Chats.Count) % (Chats.Count + 1);

        private void SelectTab(string tabIndexString)
        {
            if (!int.TryParse(tabIndexString, out var tabIndex))
            {
                return;
            }

            SelectedTabIndex = tabIndex;
        }

        private bool CanSelectTab(string tabIndexString) => int.TryParse(tabIndexString, out var tabIndex) && tabIndex >= 0 && tabIndex <= Chats.Count;

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

        private bool ProcessClientCommand()
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
                    var chat = GetOrCreateChat(target);
                    FocusChat(chat);
                }

                TextMessage = null;

                return true;
            }

            return false;
        }

        private void FocusChat(ChatViewModel chat)
        {
            SelectedTab = chat;
        }

        private void IrcClient_MessageSent(object sender, Message message)
        {
            DrawMessage(message);
        }

        private void IrcClient_MessageReceived(object sender, Message message)
        {
            DrawMessage(message);
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

        private void DrawMessage(Message message)
        {
            if (message is PrivMsgMessage privMsgMessage)
            {
                var now = TimeOnly.FromDateTime(DateTime.Now);
                string from;
                string target;
                if (privMsgMessage.From is null)
                {
                    from = Nickname;
                    target = privMsgMessage.Target;
                }
                else
                {
                    from = GetNickName(privMsgMessage.From);
                    target = privMsgMessage.Target.Equals(Nickname, StringComparison.InvariantCultureIgnoreCase)
                        ? from
                        : privMsgMessage.Target;
                }

                var text = $"[{now}] <{from}> {privMsgMessage.Text}";
                DrawMessageToTarget(target, text);
            }
            else if (message is JoinMessage joinMessage)
            {
                ChannelViewModel channel = null;
                if (joinMessage.From is null)
                {
                    channel = (ChannelViewModel)DrawMessageToTarget(joinMessage.ChannelName, $"Now talking in {joinMessage.ChannelName}");
                    FocusChat(channel);

                    Irc.Join(joinMessage.ChannelName, nickname);
                }
                else
                {
                    var from = GetNickName(joinMessage.From);
                    channel = (ChannelViewModel)DrawMessageToTarget(joinMessage.ChannelName, $"{from} has joined {joinMessage.ChannelName}");

                    Irc.Join(joinMessage.ChannelName, from);
                }

                channel.Users = new ObservableCollection<string>(Irc.Channels[joinMessage.ChannelName].Users.Keys);
            }
            else if (message is PartMessage partMessage)
            {
                if (partMessage.From is null)
                {
                    Irc.Part(partMessage.ChannelName, nickname);
                }
                else
                {
                    var from = GetNickName(partMessage.From);
                    if (from.Equals(nickname, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return;
                    }
                    DrawMessageToTarget(partMessage.ChannelName, $"{from} has left {partMessage.ChannelName}");

                    Irc.Part(partMessage.ChannelName, from);

                    var channel = (ChannelViewModel)Chats.Single(c => c.Target == partMessage.ChannelName);
                    channel.Users = new ObservableCollection<string>(Irc.Channels[partMessage.ChannelName].Users.Keys);
                }
            }
            //else if (message is TopicReply topicReply)
            //{
            //    DrawMessageToTarget(topicReply.Target, topicReply.ToString());
            //}
            //else if (message is TopicWhoTimeReply topicWhoTimeReply)
            //{
            //    DrawMessageToTarget(topicWhoTimeReply.Target, topicWhoTimeReply.ToString());
            //}
            else if (message is NameReply nameReply)
            {
                var channel = (ChannelViewModel)DrawMessageToTarget(nameReply.ChannelName, nameReply.ToString());

                Irc.Join(nameReply.ChannelName, nameReply.Nicknames);

                channel.Users = new ObservableCollection<string>(Irc.Channels[nameReply.ChannelName].Users.Keys);
            }
            //else if (message is EndOfNamesReply endOfNamesReply)
            //{
            //    DrawMessageToTarget(endOfNamesReply.Target, endOfNamesReply.ToString());
            //}
            else if (message is Reply reply)
            {
                DrawMessageToTarget(reply.Target, reply.ToString());
            }
            else if (message is QuitMessage quitMessage)
            {
                if (quitMessage.Target is not null)
                {
                    var target = GetNickName(quitMessage.Target);
                    foreach (var chat in Chats)
                    {
                        if (string.Equals(chat.Target, target, StringComparison.InvariantCultureIgnoreCase))
                        {
                            DrawMessageToTarget(target, quitMessage.Reason);
                        }
                    }

                    Irc.Quit(target);
                }
                else
                {
                    Irc.Quit(nickname);
                }
            }
            else
            {
                status.Log.Add(message.ToString());
            }
        }

        private string GetNickName(string target) => target.Split('!')[0];

        private ChatViewModel DrawMessageToTarget(string target, string message)
        {
            var chat = GetOrCreateChat(target);

            chat.Chat.Add(message);

            if (chat != SelectedTab)
            {
                chat.IsDirty = true;
            }

            return chat;
        }

        private ChatViewModel GetOrCreateChat(string target)
        {
            var chat = chats.SingleOrDefault(chat => chat.Target is not null && chat.Target.Equals(target, StringComparison.InvariantCultureIgnoreCase));
            if (chat is null)
            {
                chat = target.StartsWith('#')
                    ? new ChannelViewModel(target)
                    : new ChatViewModel(target);

                chats.Add(chat);
            }

            return chat;
        }

        public ConnectionState State
        {
            get => state;
            set => SetProperty(ref state, value);
        }

        public string Host
        {
            get => host;
            set => SetProperty(ref host, value);
        }

        public string Nickname
        {
            get => nickname;
            set => SetProperty(ref nickname, value);
        }

        public StatusViewModel Status
        {
            get => status;
            set => SetProperty(ref status, value);
        }

        public ObservableCollection<ChatViewModel> Chats
        {
            get => chats;
            set => SetProperty(ref chats, value);
        }

        public object SelectedTab
        {
            get => selectedTab;
            set => SetProperty(ref selectedTab, value);
        }

        public int SelectedTabIndex
        {
            get => selectedTabIndex;
            set => SetProperty(ref selectedTabIndex, value);
        }

        public string TextMessage
        {
            get => textMessage;
            set => SetProperty(ref textMessage, value);
        }

        public bool IsTextMessageFocused
        {
            get => isTextMessageFocused;
            set => SetProperty(ref isTextMessageFocused, value);
        }
    }
}
