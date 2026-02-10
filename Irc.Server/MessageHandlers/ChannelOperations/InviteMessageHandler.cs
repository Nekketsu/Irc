using Irc.Messages.Messages;
using Messages.Replies.CommandResponses;
using Messages.Replies.ErrorReplies;

namespace Irc.Server.MessageHandlers.ChannelOperations;

public class InviteMessageHandler : MessageHandler<InviteMessage>
{
    public async override Task<bool> HandleAsync(InviteMessage message, IrcClient ircClient)
    {
        var invitedClient = ircClient.IrcServer.IrcClients.SingleOrDefault(client => client.Profile.Nickname.Equals(message.Nickname, StringComparison.OrdinalIgnoreCase));
        if (invitedClient is null)
        {
            await ircClient.WriteMessageAsync(new NoSuchNickError(ircClient.IrcServer.ServerName, ircClient.Profile.Nickname, message.Nickname, NoSuchNickError.DefaultMessage));
            return true;
        }
        if (!ircClient.Channels.ContainsKey(message.ChannelName))
        {
            await ircClient.WriteMessageAsync(new NotOnChannelError(ircClient.IrcServer.ServerName, ircClient.Profile.Nickname, message.ChannelName, NotOnChannelError.DefaultMessage));
            return true;
        }
        if (invitedClient.Channels.ContainsKey(message.ChannelName))
        {
            await ircClient.WriteMessageAsync(new UserOnChannelError(ircClient.IrcServer.ServerName, ircClient.Profile.Nickname, invitedClient.Profile.Nickname, message.ChannelName, UserOnChannelError.DefaultMessage));
            return true;
        }

        await invitedClient.WriteMessageAsync(new InviteMessage(ircClient.Profile.Nickname, message.Nickname, message.ChannelName));

        await ircClient.WriteMessageAsync(new InvitingReply(ircClient.IrcServer.ServerName, ircClient.Profile.Nickname, message.ChannelName, message.Nickname));
        // if (invitedClient.IsAway)
        // {
        //     await ircClient.WriteMessageAsync(new AwayReply());
        // }



        return true;
    }
}
