using Irc.Messages.Messages;
using Messages.Replies.CommandResponses;

namespace Irc.Server.MessageHandlers.ChannelOperations
{
    public class ChannelModeMessageHandler : MessageHandler<ChannelModeMessage>
    {
        public async override Task<bool> HandleAsync(ChannelModeMessage message, IrcClient ircClient)
        {
            await ircClient.WriteMessageAsync(new ChannelModeIsReply(ircClient.IrcServer.ServerName, ircClient.Profile.Nickname, message.ChannelName, "+nt"));

            return true;
        }
    }
}
