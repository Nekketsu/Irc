using System.Linq;
using System.Threading.Tasks;
using Messages.Replies.CommandResponses;
using Messages.Replies.ErrorReplies;

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
            Message = message?.StartsWith(':') ?? false
                ? message.Substring(1)
                : message;
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

            if (Message != null)
            {
                text = $"{text} :{Message}";
            }

            return text;
        }

        public new static PartMessage Parse(string message)
        {
            var messageSplit = message.Split();
            var channelName = messageSplit[1];

            var text = message.Substring(message.IndexOf(messageSplit[0]) + messageSplit[0].Length).TrimStart();
            text = message.Substring(message.IndexOf(messageSplit[1]) + messageSplit[1].Length).TrimStart();

            if (text.Length == 0)
            {
                text = null;
            }

            return new PartMessage(channelName, text);
        }
    }
}