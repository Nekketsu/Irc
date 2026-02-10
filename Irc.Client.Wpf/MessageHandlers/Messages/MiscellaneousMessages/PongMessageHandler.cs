using Irc.Messages.Messages;

namespace Irc.Client.Wpf.MessageHandlers.Messages.MiscellaneousMessages;

public class PongMessageHandler : IMessageHandler<PongMessage>
{
    public Task HandleAsync(PongMessage message)
    {
        return Task.CompletedTask;
    }
}
