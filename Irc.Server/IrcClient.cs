using Irc.Domain.Extensions;
using Irc.Messages;
using Irc.Messages.Messages;
using Irc.Server.MessageHandlers;
using Messages.Replies.CommandResponses;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;

namespace Irc.Server
{
    public class IrcClient
    {
        public ILogger Logger => IrcServer.Logger;

        public IrcServer IrcServer { get; set; }
        TcpClient tcpClient;
        private EndPoint localEndPoint;
        private EndPoint remoteEndPoint;
        StreamReader streamReader;
        StreamWriter streamWriter;
        public Profile Profile { get; private set; }
        public IPAddress Address { get; private set; }

        public PingMessage PingMessage { get; private set; }
        public DateTime LastMessageDateTime { get; private set; }
        public DateTime SignOn { get; set; }

        public Dictionary<string, Channel> Channels { get; private set; }

        public IrcClient(IrcServer ircServer, TcpClient tcpClient)
        {
            IrcServer = ircServer;

            this.tcpClient = tcpClient;
            localEndPoint = tcpClient.Client.LocalEndPoint;
            remoteEndPoint = tcpClient.Client.RemoteEndPoint;
            var stream = tcpClient.GetStream();
            streamReader = new StreamReader(stream);
            streamWriter = new StreamWriter(stream) { AutoFlush = true };

            Address = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address;

            var now = DateTime.Now;
            LastMessageDateTime = now;
            SignOn = now;

            Profile = new Profile();
            Channels = new Dictionary<string, Channel>(StringComparer.OrdinalIgnoreCase);
        }

        public async void RunAsync(CancellationToken stoppingToken)
        {
            Logger.LogDebug($"Client connected: {localEndPoint}, {remoteEndPoint}");
            try
            {
                if (!await HandShake()) return;

                var pingCancellationTokenSource = new CancellationTokenSource();
                DoPeriodicPingAsync(TimeSpan.FromMinutes(1), pingCancellationTokenSource.Token);

                var isRunning = true;
                while (isRunning && !stoppingToken.IsCancellationRequested)
                {
                    var message = await ReadMessageAsync().WithCancellation(stoppingToken);
                    isRunning = await HandleMessageAsync(message).WithCancellation(stoppingToken);
                }
                pingCancellationTokenSource.Cancel();
            }
            catch { }
            finally
            {
                tcpClient.Dispose();
            }

            var disconnectLog = $"Client disconnectd: {localEndPoint}, {remoteEndPoint}";
            if (!string.IsNullOrEmpty(Profile.Nickname))
            {
                disconnectLog += $" ({Profile.Nickname})";
            }
            Logger.LogDebug(disconnectLog);
        }

        private async Task<bool> HandShake()
        {
            Message message;
            do
            {
                message = await ReadMessageAsync();
            } while (message is not NickMessage);
            await HandleMessageAsync(message);

            message = await ReadMessageAsync();
            if (message is not UserMessage)
            {
                return false;
            }
            await HandleMessageAsync(message);

            // await SendMessageAsync(new NoticeMessage("AUTH", "*** Looking up your hostname"));
            // await SendMessageAsync(new NoticeMessage("AUTH", "*** Checking Ident"));

            await WriteMessageAsync(new WelcomeReply(IrcServer.ServerName, Profile.Nickname, "IRC Network", Profile.Nickname));
            await WriteMessageAsync(new YourHostReply(IrcServer.ServerName, Profile.Nickname, IrcServer.ServerName, IrcServer.Version.ToString()));
            await WriteMessageAsync(new CreatedReply(IrcServer.ServerName, Profile.Nickname, IrcServer.CreatedDateTime));
            await WriteMessageAsync(new MyInfoReply(IrcServer.ServerName, Profile.Nickname, IrcServer.ServerName, IrcServer.Version.ToString(), "diOoswkgx", "biklmnopstvrDdRcC", "bklov"));
            PingMessage = new PingMessage($"{DateTimeOffset.Now.ToUnixTimeSeconds()}");
            await WriteMessageAsync(PingMessage);

            return true;
        }

        private async void DoPeriodicPingAsync(TimeSpan delay, CancellationToken cancellationToken)
        {
            try
            {
                do
                {
                    await Task.Delay(delay, cancellationToken);
                    PingMessage = new PingMessage($"{DateTimeOffset.Now.ToUnixTimeSeconds()}");
                    await WriteMessageAsync(PingMessage, cancellationToken);
                } while (!cancellationToken.IsCancellationRequested);
            }
            catch { }
        }

        public async Task WriteMessageAsync(IMessage message)
        {
            var source = IrcServer.ServerName;
            var destination = Profile.Nickname ?? Address.ToString();

            await streamWriter.WriteLineAsync(message.ToString());
            Logger.LogInformation($"-> {destination}: {message}");
        }

        public async Task WriteMessageAsync(IMessage message, CancellationToken cancellationToken)
        {
            var destination = Profile.Nickname ?? Address.ToString();

            await streamWriter.WriteLineAsync(message.ToString().AsMemory(), cancellationToken);
            Logger.LogInformation($"-> {destination}: {message}");
        }

        public async Task<Message> ReadMessageAsync()
        {
            var text = await streamReader.ReadLineAsync();
            LastMessageDateTime = DateTime.Now;

            var message = Message.Parse(text);

            var source = Profile.Nickname ?? Address.ToString();

            if (message is not null)
            {
                Logger.LogInformation($"<- {source}: {text}");
            }
            else
            {
                Logger.LogWarning($"<- {source}: {text}");
            }

            return message;
        }

        private async Task<bool> HandleMessageAsync(Message message)
        {
            return await MessageHandler.HandleAsync(message, this);
        }
    }
}