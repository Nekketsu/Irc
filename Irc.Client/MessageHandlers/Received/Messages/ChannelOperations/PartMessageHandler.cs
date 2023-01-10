using Irc.Messages.Messages;

namespace Irc.Client.MessageHandlers.Received.Messages
{
    internal class PartMessageHandler : IMessageHandler<PartMessage>
    {
        public void Handle(PartMessage message, IrcClient ircClient)
        {
            User from = message.From;

            ircClient.Part(message.ChannelName, from.Nickname);

            var channel = ircClient.Channels[message.ChannelName];
            channel.OnUserJoined(from.Nickname);
        }
    }
}
