using Irc.Client.Wpf.Converters;
using Irc.Client.Wpf.Model;
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
using System.Windows.Input;

namespace Irc.Client.Wpf.ViewModels
{
    public class IrcViewModel : BindableBase
    {
        private ConnectionState state;
        private string host;
        private string nickname;

        private ChatViewModel status;
        private ObservableCollection<ChatViewModel> chats;
        private ChatViewModel selectedChat;
        private string textMessage;

        private bool isTextMessageFocused;

        public ICommand ConnectCommand { get; set; }
        public ICommand SendCommand { get; set; }

        private IrcClient ircClient;
        private CancellationTokenSource cancellationTokenSource;

        public IrcViewModel()
        {
            State = ConnectionState.Disconnected;
            Host = "irc.irc-hispano.org";
            Nickname = "Nekketsu";

            status = new ChatViewModel("Status");
            Chats = new ObservableCollection<ChatViewModel>
            {
                status
            };

            IsTextMessageFocused = true;


            ConnectCommand = new DelegateCommand(ConnectAsync, () => State == ConnectionState.Disconnected && !string.IsNullOrEmpty(Host) && !string.IsNullOrEmpty(Nickname))
                .ObservesProperty(() => State)
                .ObservesProperty(() => Host)
                .ObservesProperty(() => Nickname);

            SendCommand = new DelegateCommand(SendAsync, () => !string.IsNullOrEmpty(TextMessage))
                .ObservesProperty(() => TextMessage);


            PropertyChanged += IrcViewModel_PropertyChanged;
        }

        private void IrcViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedChat))
            {
                SelectedChat.IsDirty = false;
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
        }

        private void Disconnect()
        {
            ircClient.MessageReceived -= IrcClient_MessageReceived;
            ircClient.MessageSent -= IrcClient_MessageSent;
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
                : new PrivMsgMessage(SelectedChat?.Target, TextMessage);

            if (message is null)
            {
                status.Chat.Add($"Error - Message not found: {TextMessage}");
                return;
            }

            await ircClient.SendMessageAsync(message);

            TextMessage = null;
        }

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
                    var chat = GetOrCreateChatByTarget(target);
                    FocusChat(chat);
                }

                TextMessage = null;

                return true;
            }

            return false;
        }

        private void FocusChat(ChatViewModel chat)
        {
            SelectedChat = chat;
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
                    from = privMsgMessage.From.Split('!')[0];
                    target = from;
                }
                var text = $"[{now}] <{from}> {privMsgMessage.Text}";

                var chat = DrawMessageToTarget(target, text);
            }
            else if (message is JoinMessage joinMessage)
            {
                var chat = DrawMessageToTarget(joinMessage.ChannelName, joinMessage.ToString());
                if (SelectedChat != chat)
                {
                    FocusChat(chat);
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
                DrawMessageToTarget(nameReply.ChannelName, nameReply.ToString());
            }
            //else if (message is EndOfNamesReply endOfNamesReply)
            //{
            //    DrawMessageToTarget(endOfNamesReply.Target, endOfNamesReply.ToString());
            //}
            else if (message is Reply reply)
            {
                DrawMessageToTarget(reply.Target, reply.ToString());
            }
            else
            {
                status.Chat.Add(message.ToString());
            }
        }

        private ChatViewModel DrawMessageToTarget(string target, string message)
        {
            var chat = GetOrCreateChatByTarget(target);

            chat.Chat.Add(message);

            if (chat != SelectedChat)
            {
                chat.IsDirty = true;
            }

            return chat;
        }

        private ChatViewModel GetOrCreateChatByTarget(string target)
        {
            var chat = chats.SingleOrDefault(chat => chat.Target is not null && chat.Target.Equals(target, StringComparison.InvariantCultureIgnoreCase));
            if (chat is null)
            {
                chat = new ChatViewModel(target, target);
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

        public ObservableCollection<ChatViewModel> Chats
        {
            get => chats;
            set => SetProperty(ref chats, value);
        }

        public ChatViewModel SelectedChat
        {
            get => selectedChat;
            set => SetProperty(ref selectedChat, value);
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
