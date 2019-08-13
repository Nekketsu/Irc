using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Irc.Extensions;
using Irc.Messages;
using Irc.Messages.Messages;
using Messages.Replies.CommandResponses;

namespace Irc
{
    public class IrcClient
    {
        public static IrcServer IrcServer { get; set; }
        TcpClient tcpClient;
        public StreamReader StreamReader { get; set; }
        public StreamWriter StreamWriter { get; set; }
        public Profile Profile {get; set; }
        public IPAddress Address { get; set; }

        public PingMessage PingMessage { get; set; }

        public IrcClient(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
            var stream = tcpClient.GetStream();
            StreamReader = new StreamReader(stream);
            StreamWriter = new StreamWriter(stream) { AutoFlush = true };

            Address = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address;

            Profile = new Profile();
        }

        public async void RunAsync()
        {
            try
            {
                if (!await HandShake()) return;

                var cancellationTokenSource = new CancellationTokenSource();
                DoPeriodicPingAsync(TimeSpan.FromMinutes(1), cancellationTokenSource.Token);

                Message message;
                var isRunning = true;
                do
                {
                    message = await StreamReader.ReadMessageAsync();
                    isRunning = await (message?.ManageMessageAsync(this) ?? Task.FromResult(false));
                } while (isRunning);
                cancellationTokenSource.Cancel();
            }
            finally
            {
                tcpClient.Dispose();
            }
        }

        private async Task<bool> HandShake()
        {
            Message message;
            do
            {
                message = await StreamReader.ReadMessageAsync();
            } while (!(message is NickMessage));
            await message.ManageMessageAsync(this);
            
            message = await StreamReader.ReadMessageAsync();
            if (!(message is UserMessage userMessage))
            {
                return false;
            }
            await message.ManageMessageAsync(this);

            // await streamWriter.WriteMessageAsync(new NoticeMessage("AUTH", "*** Looking up your hostname"));
            // await streamWriter.WriteMessageAsync(new NoticeMessage("AUTH", "*** Checking Ident"));

            await StreamWriter.WriteMessageAsync(new WelcomeReply($"{Profile.NickName} :Welcome to the IRC Network, {Profile.NickName}"));
            await StreamWriter.WriteMessageAsync(new YourHostReply($"{Profile.NickName} :Your host is {IrcServer.HostName}, running version {IrcServer.Version}"));
            await StreamWriter.WriteMessageAsync(new CreatedReply($"{Profile.NickName} :This server was created {IrcServer.CreatedDateTime}"));
            await StreamWriter.WriteMessageAsync(new MyInfoReply($"{Profile.NickName} {IrcServer.HostName} u2.10.12.19 diOoswkgx biklmnopstvrDdRcC bklov"));

            return true;
        }

        private async void DoPeriodicPingAsync(TimeSpan delay, CancellationToken cancellationToken)
        {
            try
            {
                do
                {
                    await Task.Delay(delay, cancellationToken);
                    PingMessage = new PingMessage($"{DateTime.Now.Ticks / TimeSpan.TicksPerSecond}");
                    await StreamWriter.WriteMessageAsync(PingMessage, cancellationToken);
                } while (!cancellationToken.IsCancellationRequested);
            } catch { };
        }
    }
}