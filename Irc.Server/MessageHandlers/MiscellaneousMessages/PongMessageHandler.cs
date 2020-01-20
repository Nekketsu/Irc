using Irc.Messages.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Server.MessageHandlers.MiscellaneousMessages
{
    public class PongMessageHandler : MessageHandler<PongMessage>
    {
        public override Task<bool> HandleAsync(PongMessage message, IrcClient ircClient)
        {
            return Task.FromResult(ircClient.PingMessage.Server == message.Server);
        }
    }
}
