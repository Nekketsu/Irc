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

        public override async Task<bool> ManageMessageAsync(IrcClient ircClient)
        {
            if (IrcClient.IrcServer.Channels.TryGetValue(ChannelName, out var channel))
            {
                var nickNames = channel.IrcClients.Select(client => client.Profile.NickName).ToArray();
                await ircClient.WriteMessageAsync(new NameReply(ircClient.Profile.NickName, channel.Name, nickNames));
                await ircClient.WriteMessageAsync(new EndOfNamesReply(ircClient.Profile.NickName, channel.Name, EndOfNamesReply.DefaultMessage));
            }

            return true;
        }
    }
}