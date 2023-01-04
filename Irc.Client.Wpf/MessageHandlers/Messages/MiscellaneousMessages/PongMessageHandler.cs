using Irc.Messages.Messages;
using System.Threading.Tasks;

namespace Irc.Client.Wpf.MessageHandlers.Messages.MiscellaneousMessages
{
    public class PongMessageHandler : IMessageHandler<PongMessage>
    {
        public Task HandleAsync(PongMessage message)
        {
            return Task.CompletedTask;
        }
    }
}
