using Irc.Messages.Messages;

namespace Irc.Server.MessageHandlers.ConnectionRegistration
{
    public class NickMessageHandler : MessageHandler<NickMessage>
    {
        public override Task<bool> HandleAsync(NickMessage message, IrcClient ircClient)
        {
            ircClient.Profile.Nickname = message.Nickname;
            return Task.FromResult(true);
        }
    }
}
