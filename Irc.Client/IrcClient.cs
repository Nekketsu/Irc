using Irc.Client.MessageHandlers;
using Irc.Domain.Extensions;
using Irc.Messages;
using Irc.Messages.Messages;
using System.Net.Sockets;

namespace Irc.Client
{
    public class IrcClient
    {
        public LocalUser LocalUser { get; }
        public UserCollection Users { get; }
        public ChannelCollection Channels { get; }

        public string Host { get; }
        public int Port { get; }

        public event EventHandler Connected;
        public event EventHandler Disconnected;
        public event EventHandler<Message> MessageReceived;
        public event EventHandler<Message> MessageSent;
        public event EventHandler<string> RawMessageReceived;
        public event EventHandler<string> RawMessageSent;

        private TcpClient tcpClient;
        private StreamReader streamReader;
        private StreamWriter streamWriter;

        public IrcClient(string nickname, string host, int port = Protocol.DefaultPort)
        {
            Host = host;
            Port = port;

            LocalUser = new LocalUser { Nickname = nickname };
            Users = new();
            Channels = new();
        }

        public async Task RunAsync(CancellationToken stoppingToken)
        {
            tcpClient = new TcpClient();

            await tcpClient.ConnectAsync(Host, Port).WithCancellation(stoppingToken);
            var stream = tcpClient.GetStream();
            streamReader = new StreamReader(stream);
            streamWriter = new StreamWriter(stream) { AutoFlush = true };

            Connected?.Invoke(this, EventArgs.Empty);
            Users[LocalUser.Nickname] = LocalUser;

            await HandShake();
            await HandleMessagesAsync(stoppingToken);
        }

        private async Task HandShake()
        {
            await SendMessageAsync(new NickMessage((string)LocalUser.Nickname));
            await SendMessageAsync(new UserMessage((string)LocalUser.Nickname, (string)LocalUser.Nickname, Host, (string)LocalUser.Nickname));

            PingMessage pingMessage;
            do
            {
                var message = await ReadMessageAsync();
                pingMessage = message as PingMessage;
            } while (pingMessage is null);

            var pongMessage = new PongMessage(pingMessage.Server);
            await SendMessageAsync(pongMessage);
        }

        public async Task SendMessageAsync<T>(T message) where T : Message
        {
            var text = message.ToString();
            await streamWriter.WriteLineAsync(text);

            if (!tcpClient.Connected)
            {
                Disconnected?.Invoke(this, new EventArgs());
                return;
            }

            RawMessageSent?.Invoke(this, text);
            MessageSent?.Invoke(this, message);
        }

        private async Task<Message> ReadMessageAsync()
        {
            var text = await streamReader.ReadLineAsync();

            if (text is null)
            {
                return null;
            }

            if (!tcpClient.Connected)
            {
                Disconnected?.Invoke(this, new EventArgs());
                return null;
            }

            RawMessageReceived?.Invoke(this, text);

            var message = Message.Parse(text);
            IMessageHandler.Handle(message, this);

            MessageReceived?.Invoke(this, message);

            return message;
        }

        private async Task HandleMessagesAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Message message = await ReadMessageAsync();

                if (message is PingMessage pingMessage)
                {
                    var pongMessage = new PongMessage(pingMessage.Server);
                    await SendMessageAsync(pongMessage);
                }
            }
        }

        internal void Join(string channelName, Nickname nickname)
        {
            var user = Users[nickname];
            if (user is null)
            {
                user = new User { Nickname = nickname };
                Users[nickname] = user;
            }

            user.Nickname = nickname;

            var channel = Channels[channelName];
            if (channel is null)
            {
                channel = new Channel { Name = channelName };
                Channels[channelName] = channel;
            }

            user.Channels[channelName] = channel;
            channel.Users[nickname] = user;
        }

        internal void Part(string channelName, Nickname nickname)
        {
            var channel = Channels[channelName];
            channel.Users.Remove(nickname);

            var user = Users[nickname];

            user.Channels.Remove(channelName);

            if (!user.Channels.Any())
            {
                Users.Remove(nickname);
            }
        }

        internal void RenameUser(Nickname previousNickname, Nickname nickname)
        {
            var user = Users[previousNickname];
            Users.Remove(previousNickname);

            user.Nickname = nickname;
            Users[nickname] = user;

            foreach (var channel in user.Channels)
            {
                channel.Users.Remove(previousNickname);
                channel.Users[nickname] = user;
            }

            user.OnNicknameChanged(previousNickname, nickname);
        }
    }
}
