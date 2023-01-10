using Irc.Client.Wpf.ViewModels;
using Irc.Client.Wpf.ViewModels.Tabs.Messages;
using Irc.Messages.Replies.CommandResponses;

namespace Irc.Client.Wpf.MessageHandlers.Replies.CommandResponses
{
    public class UnAwayReplyMessageHandler : IMessageHandler<UnAwayReply>
    {
        private readonly IrcViewModel viewModel;

        public UnAwayReplyMessageHandler(IrcViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public Task HandleAsync(UnAwayReply message)
        {
            var text = message.Message;

            var messageViewModel = new MessageViewModel(text);
            viewModel.DrawMessage(viewModel.Status, messageViewModel);

            return Task.CompletedTask;
        }
    }
}
