using Irc.Messages;

namespace Irc.Server.MessageHandlers
{
    public class MessageHandler<T> where T : Message
    {
        public virtual Task<bool> HandleAsync(T message, IrcClient ircClient)
        {
            return Task.FromResult(true);
        }
    }
}
