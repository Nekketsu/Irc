using System;
using System.Threading.Tasks;

namespace Irc.Messages.Messages
{
    public class QuitMessage : Message
    {
        public string Target { get; set; }
        public string Reason { get; set; }

        public QuitMessage(string reason)
        {
            Reason = reason;
        }

        public QuitMessage(string target, string reason) : this(reason)
        {
            Target = target;
        }

        public override string ToString()
        {
            return (Target == null)
                ? $"{Command} :{Reason}"
                : $":{Target} {Command} :{Reason}";
        }

        public override async Task<bool> ManageMessageAsync(IrcClient ircClient)
        {
            var quitMessage = new QuitMessage(ircClient.Profile.NickName, $"Quit: {Reason}");
            foreach (var channel in ircClient.Channels.Values)
            {
                foreach (var client in channel.IrcClients)
                {
                    await client.WriteMessageAsync(quitMessage);
                }

                channel.IrcClients.Remove(ircClient);
                ircClient.Channels.Remove(channel.Name);
            }

            Console.WriteLine($"{ircClient.Profile.NickName} has disconnected IRC.");

            return false;
        }

        public new static QuitMessage Parse(string message)
        {
            var messageSplit = message.Split();

            var reason = message.Substring(message.IndexOf(messageSplit[0]) + messageSplit[0].Length).TrimStart();
            if (reason.StartsWith(':'))
            {
                reason = reason.Substring(1);
            }

            return new QuitMessage(reason);
        }
    }
}