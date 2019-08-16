using System.Linq;
using System.Threading.Tasks;
using Messages.Replies.CommandResponses;

namespace Irc.Messages.Messages
{
    public class PartMessage : Message
    {
        public string From { get; set; }
        public string ChannelName { get; set; }
        public string Message { get; set; }

        public PartMessage(string channelName, string message)
        {
            ChannelName = channelName;
            Message = message;
        }

        public PartMessage(string from, string channelName, string message) : this(channelName, message)
        {
            From = from;
        }

        public override string ToString()
        {
            var text = (From == null)
                ? $"{Command} {ChannelName}"
                : $":{From} {Command} {ChannelName}";

            if (!string.IsNullOrEmpty(Message))
            {
                text = $"{text} :{Message}";
            }

            return text;
        }

        public override async Task<bool> ManageMessageAsync(IrcClient ircClient)
        {
            if (ircClient.Channels.TryGetValue(ChannelName, out var partChannel))
            {
                var partMessage = new PartMessage(ircClient.Profile.NickName, ChannelName, Message);
                foreach (var client in partChannel.IrcClients.Where(c => c != ircClient))
                {
                    await client.WriteMessageAsync(partMessage);
                }

                ircClient.Channels.Remove(ChannelName);
                partChannel.IrcClients.Remove(ircClient);
                if (!partChannel.IrcClients.Any())
                {
                    IrcClient.IrcServer.Channels.Remove(ChannelName);
                }
            }
            else if (IrcClient.IrcServer.Channels.ContainsKey(ChannelName))
            {
                await ircClient.WriteMessageAsync(new NotOnChannelReply(ircClient.Profile.NickName, ChannelName, "You're not on that channel"));
            }
            else
            {
                await ircClient.WriteMessageAsync(new NoSuchChannelReply(ircClient.Profile.NickName, ChannelName, "No such channel"));
            }

            return true;
        }

        public new static PartMessage Parse(string message)
        {
            var messageSplit = message.Split();
            var channelName = messageSplit[1];

            var text = message.Substring(message.IndexOf(messageSplit[0]) + messageSplit[0].Length).TrimStart();
            text = message.Substring(message.IndexOf(messageSplit[1]) + messageSplit[1].Length).TrimStart();

            return new PartMessage(channelName, text);
        }
    }
}