using Irc.Client.Wpf.ViewModels;
using Irc.Client.Wpf.ViewModels.Tabs.Messages;
using Messages.Replies.CommandResponses;
using System.Threading.Tasks;

namespace Irc.Client.Wpf.MessageHandlers.Replies.CommandResponses
{
    public class MotdStartReplyMessageHandler : IMessageHandler<MotdStartReply>
    {
        private readonly IrcViewModel viewModel;

        public MotdStartReplyMessageHandler(IrcViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public Task HandleAsync(MotdStartReply message)
        {
            var messageViewModel = new MessageViewModel(message.ToString());
            viewModel.DrawMessage(viewModel.Status, messageViewModel);
            viewModel.DrawMessage(viewModel.Status, new MessageViewModel("-"));

            return Task.CompletedTask;
        }
    }
}
