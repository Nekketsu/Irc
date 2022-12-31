using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Irc.Client.Wpf.Messages;
using Irc.Client.Wpf.Model;
using Irc.Client.Wpf.ViewModels.Tabs;
using Irc.Client.Wpf.ViewModels.Tabs.Messages;
using Irc.Messages;
using Irc.Messages.Messages;
using Messages.Replies.CommandResponses;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;

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
        private ObservableCollection<ChatViewModel> chats;

        [ObservableProperty]
        private object selectedTab;

        [ObservableProperty]
        private int selectedTabIndex;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SendAsyncCommand))]
        private string textMessage;


        [ObservableProperty]
        private bool isTextMessageFocused;


        private IrcClient ircClient;
        private CancellationTokenSource cancellationTokenSource;

        private Model.Irc Irc { get; }

        public IrcViewModel(IMessenger messenger)
        {
            State = ConnectionState.Disconnected;
            Host = "irc.irc-hispano.org";
            Nickname = "Nekketsu";

            Status = new();
            Chats = new();

            FocusInput();

            Irc = new();

            PropertyChanged += IrcViewModel_PropertyChanged;

            messenger.Register<QueryNicknameMessage>(this, (r, m) =>
            {
                Query(m.Nickname);
            });
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

            Irc.Users.Add(Nickname, new Model.User(Nickname));
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
                var log = new MessageViewModel($"Error - Message not found: {TextMessage}");
                Status.Log.Add(log);
                return;
            }

            await ircClient.SendMessageAsync(message);

            TextMessage = null;
        }

        private bool CanSend() => !string.IsNullOrEmpty(TextMessage);

        [RelayCommand]
        private void NextTab() => SelectedTabIndex = (SelectedTabIndex + 1) % (Chats.Count + 1);

        [RelayCommand]
        private void PreviousTab() => SelectedTabIndex = (SelectedTabIndex + Chats.Count) % (Chats.Count + 1);

        [RelayCommand(CanExecute = nameof(CanSelectTab))]
        private void SelectTab(string tabIndexString)
        {
            if (!int.TryParse(tabIndexString, out var tabIndex))
            {
                return;
            }

            SelectedTabIndex = tabIndex;
        }

        private bool CanSelectTab(string tabIndexString) => int.TryParse(tabIndexString, out var tabIndex) && tabIndex >= 0 && tabIndex <= Chats.Count;

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
                    Query(target);
                }

                TextMessage = null;

                return true;
            }

            return false;
        }

        private void Query(string target)
        {
            var chat = GetOrCreateChat(target);
            FocusChat(chat);
            FocusInput();
        }

        private void FocusChat(ChatViewModel chat)
        {
            SelectedTab = chat;
        }

        private void FocusInput()
        {
            IsTextMessageFocused = false;
            IsTextMessageFocused = true;
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

                var messageViewModel = new ChatMessageViewModel(from, privMsgMessage.Text);
                DrawMessageToTarget(target, messageViewModel);
            }
            else if (message is JoinMessage joinMessage)
            {
                ChannelViewModel channel = null;
                if (joinMessage.From is null)
                {
                    var messageViewModel = new MessageViewModel($"Now talking in {joinMessage.ChannelName}");
                    channel = (ChannelViewModel)DrawMessageToTarget(joinMessage.ChannelName, messageViewModel);
                    FocusChat(channel);

                    Irc.Join(joinMessage.ChannelName, nickname);
                }
                else
                {
                    var from = GetNickName(joinMessage.From);
                    var messageViewModel = new MessageViewModel($"{from} has joined {joinMessage.ChannelName}");
                    channel = (ChannelViewModel)DrawMessageToTarget(joinMessage.ChannelName, messageViewModel);

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
                    var messageViewModel = new MessageViewModel($"{from} has left {partMessage.ChannelName}");
                    DrawMessageToTarget(partMessage.ChannelName, messageViewModel);

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
                var messageViewModel = new MessageViewModel(nameReply.ToString());
                var channel = (ChannelViewModel)DrawMessageToTarget(nameReply.ChannelName, messageViewModel);

                Irc.Join(nameReply.ChannelName, nameReply.Nicknames);

                channel.Users = new ObservableCollection<string>(Irc.Channels[nameReply.ChannelName].Users.Keys);
            }
            //else if (message is EndOfNamesReply endOfNamesReply)
            //{
            //    DrawMessageToTarget(endOfNamesReply.Target, endOfNamesReply.ToString());
            //}
            else if (message is Reply reply)
            {
                var messageViewModel = new MessageViewModel(reply.ToString());
                DrawMessageToTarget(reply.Target, messageViewModel);
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
                            var messageViewModel = new MessageViewModel(quitMessage.Reason);
                            DrawMessageToTarget(target, messageViewModel);
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
                var messageViewModel = new MessageViewModel(message.ToString());
                status.Log.Add(messageViewModel);
            }
        }

        private string GetNickName(string target) => target.Split('!')[0];

        private ChatViewModel DrawMessageToTarget(string target, MessageViewModel message)
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
    }
}
