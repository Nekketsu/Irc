using Irc.Messages.Messages;

namespace Irc.Client.MessageHandlers.Received.Messages.SendingMessages;

internal class PrivMsgMessageHandler : IMessageHandler<PrivMsgMessage>
{
    public void Handle(PrivMsgMessage message, IrcClient ircClient)
    {
        User from = message.From;

        if (ircClient.LocalUser.Nickname == message.Target)
        {
            ircClient.LocalUser.OnMessageReceived(from.Nickname, message.Text);
        }
        else
        {
            var channel = ircClient.Channels[message.Target];
            channel.OnMessageReceived(from.Nickname, message.Text);
        }
    }
}
