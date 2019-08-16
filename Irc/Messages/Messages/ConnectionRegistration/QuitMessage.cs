using System;
using System.Threading.Tasks;

namespace Irc.Messages.Messages
{
    public class QuitMessage : Message
    {
        public string Message { get; set; }

        public QuitMessage(string message)
        {
            Message = message;
        }

        public override string ToString()
        {
            return $"{Command} {Message}";
        }

        public override Task<bool> ManageMessageAsync(IrcClient ircClient)
        {
            Console.WriteLine($"Client {ircClient.Profile.NickName} has disconnected IRC.");
            return Task.FromResult(false);
        }
    }
}