using Irc.Messages.Messages;

namespace Irc.Server.MessageHandlers.ConnectionRegistration
{
    public class UserMessageHandler : MessageHandler<UserMessage>
    {
        public override Task<bool> HandleAsync(UserMessage message, IrcClient ircClient)
        {
            ircClient.Profile.User = new User(message.User, message.Mode, message.Unused, message.RealName);
            return Task.FromResult(true);
        }
    }
}
