using System.Threading.Tasks;
using Messages.Replies.CommandResponses;

namespace Irc.Messages.Messages
{
    public class ListMessage : Message
    {
        public ListMessage()
        {
        }

        public override string ToString()
        {
            return $"{Command}";
        }
        
        public override async Task<bool> ManageMessageAsync(IrcClient ircClient)
        {
            foreach (var channel in IrcClient.IrcServer.Channels.Values)
            {
                await ircClient.WriteMessageAsync(new ListReply(ircClient.Profile.Nickname, channel.Name, channel.IrcClients.Count, channel.Topic?.TopicMessage));
            }
            await ircClient.WriteMessageAsync(new ListEndReply(ircClient.Profile.Nickname, ListEndReply.DefaultMessage));

            return true;
        }
    }
}