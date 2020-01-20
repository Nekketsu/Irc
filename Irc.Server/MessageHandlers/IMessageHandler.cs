using Irc.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
