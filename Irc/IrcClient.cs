using System;
using System.Collections.Generic;
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
        StreamReader streamReader;
        StreamWriter streamWriter;
        public Profile Profile {get; private set; }
        public IPAddress Address { get; private set; }

        public PingMessage PingMessage { get; private set; }

        public Dictionary<string, Channel> Channels { get; private set; }

        public IrcClient(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
            var stream = tcpClient.GetStream();
            streamReader = new StreamReader(stream);
            streamWriter = new StreamWriter(stream) { AutoFlush = true };

            Address = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address;

            Profile = new Profile();
            Channels = new Dictionary<string, Channel>();
        }

        public async void RunAsync()
        {
            try
            {
                if (!await HandShake()) return;

                var cancellationTokenSource = new CancellationTokenSource();
                DoPeriodicPingAsync(TimeSpan.FromMinutes(1), cancellationTokenSource.Token);

                var isRunning = true;
                do
                {
                    var message = await streamReader.ReadMessageAsync();
                    isRunning = await (message?.ManageMessageAsync(this) ?? Task.FromResult(true));
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
                message = await ReadMessageAsync();
            } while (!(message is NickMessage));
            await message.ManageMessageAsync(this);
            
            message = await ReadMessageAsync();
            if (!(message is UserMessage userMessage))
            {
                return false;
            }
            await message.ManageMessageAsync(this);

            // await SendMessageAsync(new NoticeMessage("AUTH", "*** Looking up your hostname"));
            // await SendMessageAsync(new NoticeMessage("AUTH", "*** Checking Ident"));

            await WriteMessageAsync(new WelcomeReply(Profile.NickName, $"Welcome to the IRC Network, {Profile.NickName}"));
            await WriteMessageAsync(new YourHostReply(Profile.NickName, $"Your host is {IrcServer.ServerName}, running version {IrcServer.Version}"));
            await WriteMessageAsync(new CreatedReply(Profile.NickName, $"This server was created {IrcServer.CreatedDateTime}"));
            await WriteMessageAsync(new MyInfoReply(Profile.NickName, $"{IrcServer.ServerName} {IrcServer.Version} diOoswkgx biklmnopstvrDdRcC bklov"));

            return true;
        }

        private async void DoPeriodicPingAsync(TimeSpan delay, CancellationToken cancellationToken)
        {
            try
            {
                do
                {
                    await Task.Delay(delay, cancellationToken);
                    PingMessage = new PingMessage($"{DateTime.Now.ToUnixTime()}");
                    await WriteMessageAsync(PingMessage, cancellationToken);
                } while (!cancellationToken.IsCancellationRequested);
            } catch { };
        }

        public async Task WriteMessageAsync(IMessage message)
        {
            var source = IrcServer.ServerName;
            var destination = Profile.NickName ?? Address.ToString();

            await streamWriter.WriteLineAsync(message.ToString());
            Console.WriteLine($"-> {destination}: {message}");
        }
        
        public async Task WriteMessageAsync(IMessage message, CancellationToken cancellationToken)
        {
            var source = IrcServer.ServerName;
            var destination = Profile.NickName ?? Address.ToString();

            await streamWriter.WriteLineAsync(message.ToString().AsMemory(), cancellationToken);
            Console.WriteLine($"-> {destination}: {message}");
        }

        public async Task<Message> ReadMessageAsync()
        {
            var text = await streamReader.ReadLineAsync();
            var message = Message.Parse(text);

            var source = Profile.NickName ?? Address.ToString();
            var destination = IrcServer.ServerName;

            if (message != null)
            {
                Console.WriteLine($"<- {source}: {text}");
            }
            else
            {
                var foregroundColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"{source}: {text}");
                Console.ForegroundColor = foregroundColor;
            }

            return message;
        }
    }
}