using Irc.Messages.Messages;

namespace Irc.Client.MessageHandlers.Received.Messages
{
    internal class JoinMessageHandler : IMessageHandler<JoinMessage>
    {
        public void Handle(JoinMessage message, IrcClient ircClient)
        {
            User user = message.From;

            ircClient.Join(message.ChannelName, user.Nickname);

            var channel = ircClient.Channels[message.ChannelName];
            channel.OnUserJoined(user.Nickname);
        }
    }
}
