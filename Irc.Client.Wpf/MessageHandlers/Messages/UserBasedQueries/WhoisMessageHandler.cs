using Irc.Messages.Messages;
using System.Threading.Tasks;

namespace Irc.Client.Wpf.MessageHandlers.Messages.UserBasedQueries
{
    public class WhoisMessageHandler : IMessageHandler<WhoisMessage>
    {
        public Task HandleAsync(WhoisMessage message)
        {
            return Task.CompletedTask;
        }
    }
}
