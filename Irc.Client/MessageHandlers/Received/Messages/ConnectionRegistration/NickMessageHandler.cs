using Irc.Messages.Messages;

namespace Irc.Client.MessageHandlers.Received.Messages;

internal class NickMessageHandler : IMessageHandler<NickMessage>
{
    public void Handle(NickMessage message, IrcClient ircClient)
    {
        User from = message.From;
        ircClient.RenameUser(from.Nickname, message.Nickname);
    }
}
