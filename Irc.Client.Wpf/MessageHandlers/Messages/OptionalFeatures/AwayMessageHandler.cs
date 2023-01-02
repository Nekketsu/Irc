using Irc.Messages.Messages.OptionalFeatures;
using System.Threading.Tasks;

namespace Irc.Client.Wpf.MessageHandlers.Messages
{
    public class AwayMessageHandler : IMessageHandler<AwayMessage>
    {
        public AwayMessageHandler()
        {
        }

        public Task HandleAsync(AwayMessage message)
        {
            return Task.CompletedTask;
        }
    }
}
