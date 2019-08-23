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

        public override async Task<bool> ManageMessageAsync(IrcClient ircClient)
        {
            await ircClient.WriteMessageAsync(new ChannelModeIsReply(ircClient.Profile.Nickname, ChannelName, "+nt"));
            
            return true;
        }
    }
}