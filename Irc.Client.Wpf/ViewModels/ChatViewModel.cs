using Irc.Client.Wpf.Model;
using Irc.Messages;
using Irc.Messages.Messages;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Windows.Input;

namespace Irc.Client.Wpf.ViewModels
{
    public class ChatViewModel : BindableBase
    {
        private ConnectionState state;
        private string host;
        private string nickname;

        private ObservableCollection<string> chat;
        private string textMessage;

        public ICommand ConnectCommand { get; set; }
        public ICommand SendCommand { get; set; }

        private IrcClient ircClient;
        private CancellationTokenSource cancellationTokenSource;

        public ChatViewModel()
        {
            State = ConnectionState.Disconnected;
            Host = "localhost";
            Nickname = "Nekketsu";
            Chat = new ObservableCollection<string>();
            ConnectAsync();

            ConnectCommand = new DelegateCommand(ConnectAsync, () => State == ConnectionState.Disconnected && !string.IsNullOrEmpty(Host) && !string.IsNullOrEmpty(Nickname))
                .ObservesProperty(() => State)
                .ObservesProperty(() => Host)
                .ObservesProperty(() => Nickname);
            SendCommand = new DelegateCommand(SendAsync, () => State == ConnectionState.Connected && !string.IsNullOrEmpty(TextMessage))
                .ObservesProperty(() => State)
                .ObservesProperty(() => TextMessage);
        }

        private async void ConnectAsync()
        {
            State = ConnectionState.Connecting;
            cancellationTokenSource = new CancellationTokenSource();

            ircClient = new IrcClient(Nickname, Host);
            ircClient.MessageReceived += IrcClient_MessageReceived;
            await ircClient.RunAsync(cancellationTokenSource.Token);

            State = ConnectionState.Connected;
        }

        private async void SendAsync()
        {
            var message = TextMessage.StartsWith("/")
                ? Message.Parse(TextMessage.Substring(1))
                : new PrivMsgMessage("Nekketsu", TextMessage);

            if (message != null)
            {
                await ircClient.SendMessageAsync(message);
            }
            else
            {
                Chat.Add($"Error - Message not found: {TextMessage}");
            }

            TextMessage = null;
        }

        private void IrcClient_MessageReceived(object sender, Message message)
        {
            Chat.Add(message.ToString());
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

        public ObservableCollection<string> Chat 
        { 
            get => chat; 
            set => SetProperty(ref chat, value);
        }

        public string TextMessage
        { 
            get => textMessage;
            set => SetProperty(ref textMessage, value);
        }
    }
}
