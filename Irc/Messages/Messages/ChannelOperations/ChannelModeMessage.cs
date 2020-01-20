using System.Threading.Tasks;
using Messages.Replies.CommandResponses;

namespace Irc.Messages.Messages
{
    public class ChannelModeMessage : Message
    {
        public string ChannelName { get; set; }

        public ChannelModeMessage(string channelName)
        {
            ChannelName = channelName;
        }

        public override string ToString()
        {
            return $"{Command} #{ChannelName}";
        }
    }
}