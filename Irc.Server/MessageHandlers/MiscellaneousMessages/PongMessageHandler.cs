using Irc.Messages.Messages;

namespace Irc.Server.MessageHandlers.MiscellaneousMessages;

public class PongMessageHandler : MessageHandler<PongMessage>
{
    public override Task<bool> HandleAsync(PongMessage message, IrcClient ircClient)
    {
        return Task.FromResult(ircClient.PingMessage.Server == message.Server);
    }
}
