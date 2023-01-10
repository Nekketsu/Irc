using Irc.Client.Wpf.ViewModels;
using Irc.Client.Wpf.ViewModels.Tabs.Messages;
using Irc.Messages.Replies.CommandResponses;

namespace Irc.Client.Wpf.MessageHandlers.Replies.CommandResponses
{
    public class NowAwayReplyMessageHandler : IMessageHandler<NowAwayReply>
    {
        private readonly IrcViewModel viewModel;

        public NowAwayReplyMessageHandler(IrcViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public Task HandleAsync(NowAwayReply message)
        {
            var text = message.Message;

            var messageViewModel = new MessageViewModel(text);
            viewModel.DrawMessage(viewModel.Status, messageViewModel);

            return Task.CompletedTask;
        }
    }
}
