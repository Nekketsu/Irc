using Irc.Messages.Messages;
using Messages.Replies.CommandResponses;

namespace Irc.Server.MessageHandlers.ChannelOperations;

public class ModeMessageHandler : MessageHandler<ModeMessage>
{
    public async override Task<bool> HandleAsync(ModeMessage message, IrcClient ircClient)
    {
        await ircClient.WriteMessageAsync(new ChannelModeIsReply(ircClient.IrcServer.ServerName, ircClient.Profile.Nickname, message.Target, "+nt"));

        return true;
    }
}
