using Irc.Domain.Extensions;
using Irc.Messages;
using Irc.Messages.Messages;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Irc.Client
{
    public class IrcClient
    {
        public string Nickname { get; }
        public string Host { get; }
        public int Port { get; }

        public event EventHandler<Message> MessageReceived;

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
        }

        public async Task SendMessageAsync<T>(T message) where T : Message
        {
            await streamWriter.WriteLineAsync(message.ToString());
        }

        private async void HandleMessagesAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var text = await streamReader.ReadLineAsync();
                var message = Message.Parse(text) ??
                              new NoticeMessage(Host, text);

                MessageReceived?.Invoke(this, message);
            }
        }


    }
}
