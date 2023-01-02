using Irc.Client.Wpf.ViewModels;
using Irc.Client.Wpf.ViewModels.Tabs.Messages;
using Irc.Messages.Messages;
using System.Threading.Tasks;

namespace Irc.Client.Wpf.MessageHandlers.Messages.UserBasedQueries
{
    public class WhoisMessageHandler : IMessageHandler<WhoisMessage>
    {
        private readonly IrcViewModel viewModel;

        public WhoisMessageHandler(IrcViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public Task HandleAsync(WhoisMessage message)
        {
            var messageViewModel = new MessageViewModel(message.ToString());
            viewModel.DrawMessage(messageViewModel);

            return Task.CompletedTask;
        }
    }
}
