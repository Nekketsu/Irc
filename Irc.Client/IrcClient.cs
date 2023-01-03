using Irc.Domain.Extensions;
using Irc.Messages;
using Irc.Messages.Messages;
using System.Net.Sockets;

namespace Irc.Client
{
    public class IrcClient
    {
        public string Nickname { get; }
        public string Host { get; }
        public int Port { get; }

        public event EventHandler<Message> MessageReceived;
        public event EventHandler<Message> MessageSent;
        public event EventHandler<string> RawMessageReceived;
        public event EventHandler<string> RawMessageSent;

        TcpClient tcpClient;
        private StreamReader streamReader;
        private StreamWriter streamWriter;

        public IrcClient(string nickname, string host, int port = Protocol.DefaultPort)
        {
            Nickname = nickname;
            Host = host;
            Port = port;
        }

        public async Task RunAsync(CancellationToken stoppingToken)
        {
            tcpClient = new TcpClient();

            await tcpClient.ConnectAsync(Host, Port).WithCancellation(stoppingToken);
            var stream = tcpClient.GetStream();
            streamReader = new StreamReader(stream);
            streamWriter = new StreamWriter(stream) { AutoFlush = true };

            await HandShake();

            HandleMessagesAsync(stoppingToken);
        }

        private async Task HandShake()
        {
            await SendMessageAsync(new NickMessage(Nickname));
            await SendMessageAsync(new UserMessage(Nickname, Nickname, Host, Nickname));

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
            RawMessageSent?.Invoke(this, text);

            await streamWriter.WriteLineAsync(text);
            MessageSent?.Invoke(this, message);
        }

        private async Task<Message> ReadMessageAsync()
        {
            var text = await streamReader.ReadLineAsync();
            RawMessageReceived?.Invoke(this, text);

            var message = Message.Parse(text);
            MessageReceived?.Invoke(this, message);

            return message;
        }

        private async void HandleMessagesAsync(CancellationToken stoppingToken)
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
    }
}
