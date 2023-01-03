using Irc.Client.Wpf.ViewModels;
using Irc.Client.Wpf.ViewModels.Tabs.Messages;
using Messages.Replies.CommandResponses;
using System.Threading.Tasks;

namespace Irc.Client.Wpf.MessageHandlers.Replies.CommandResponses
{
    public class MotdReplyMessageHandler : IMessageHandler<MotdReply>
    {
        private readonly IrcViewModel viewModel;

        public MotdReplyMessageHandler(IrcViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public Task HandleAsync(MotdReply message)
        {
            var messageViewModel = new MessageViewModel(message.ToString());
            viewModel.DrawMessage(viewModel.Status, messageViewModel);

            return Task.CompletedTask;
        }
    }
}
