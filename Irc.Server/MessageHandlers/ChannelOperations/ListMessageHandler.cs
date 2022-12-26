using Irc.Messages.Messages;
using Messages.Replies.CommandResponses;

namespace Irc.Server.MessageHandlers.ChannelOperations
{
    public class ListMessageHandler : MessageHandler<ListMessage>
    {
        public async override Task<bool> HandleAsync(ListMessage message, IrcClient ircClient)
        {
            foreach (var channel in ircClient.IrcServer.Channels.Values)
            {
                await ircClient.WriteMessageAsync(new ListReply(ircClient.IrcServer.ServerName, ircClient.Profile.Nickname, channel.Name, channel.IrcClients.Count, channel.Topic?.TopicMessage));
            }
            await ircClient.WriteMessageAsync(new ListEndReply(ircClient.IrcServer.ServerName, ircClient.Profile.Nickname, ListEndReply.DefaultMessage));

            return true;
        }
    }
}
