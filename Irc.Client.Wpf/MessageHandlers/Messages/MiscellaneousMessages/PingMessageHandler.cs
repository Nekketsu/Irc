using Irc.Messages.Messages;
using System.Threading.Tasks;

namespace Irc.Client.Wpf.MessageHandlers.Messages.MiscellaneousMessages
{
    public class PingMessageHandler : IMessageHandler<PingMessage>
    {
        public Task HandleAsync(PingMessage message)
        {
            return Task.CompletedTask;
        }
    }
}
