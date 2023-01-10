using Irc.Messages.Messages;

namespace Irc.Client.MessageHandlers.Received.Messages
{
    internal class QuitMessageHandler : IMessageHandler<QuitMessage>
    {
        public void Handle(QuitMessage message, IrcClient ircClient)
        {
            User from = message.Target;

            var user = ircClient.Users[from.Nickname];

            foreach (var channel in user.Channels)
            {
                channel.Users.Remove(user.Nickname);
            }

            ircClient.Users.Remove(user.Nickname);

            user.OnQuit(message.Reason);
        }
    }
}
