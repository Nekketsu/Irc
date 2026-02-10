using Irc.Messages.Messages;

namespace Irc.Client.Wpf.MessageHandlers.Messages.MiscellaneousMessages;

public class PingMessageHandler : IMessageHandler<PingMessage>
{
    public Task HandleAsync(PingMessage message)
    {
        return Task.CompletedTask;
    }
}
