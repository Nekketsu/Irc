using Messages.Replies.CommandResponses;

namespace Irc.Client.MessageHandlers.Received.Replies.CommandResponses;

internal class NameReplyMessageHandler : IMessageHandler<NameReply>
{
    public void Handle(NameReply message, IrcClient ircClient)
    {
        foreach (var nickname in message.Nicknames)
        {
            ircClient.Join(message.ChannelName, nickname);
        }
    }
}
