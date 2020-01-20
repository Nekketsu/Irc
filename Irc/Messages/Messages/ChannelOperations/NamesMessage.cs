using System.Linq;
using System.Threading.Tasks;
using Messages.Replies.CommandResponses;

namespace Irc.Messages.Messages
{
    public class NamesMessage : Message
    {
        public string ChannelName { get; set; }

        public NamesMessage(string channelName)
        {
            ChannelName = channelName;
        }

        public override string ToString()
        {
            return $"{Command} {ChannelName}";
        }
    }
}