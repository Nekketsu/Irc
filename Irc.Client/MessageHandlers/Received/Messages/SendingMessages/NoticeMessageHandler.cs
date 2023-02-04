using Irc.Messages.Messages;

namespace Irc.Client.MessageHandlers.Received.Messages.SendingMessages
{
    internal class NoticeMessageHandler : IMessageHandler<NoticeMessage>
    {
        public void Handle(NoticeMessage message, IrcClient ircClient)
        {
            User from = message.From;

            if (ircClient.LocalUser.Nickname == message.Target)
            {
                ircClient.LocalUser.OnNoticeReceived(from.Nickname, message.Text);
            }
            else
            {
                var channel = ircClient.Channels[message.Target];
                channel?.OnNoticeReceived(from.Nickname, message.Text);
            }
        }
    }
}
