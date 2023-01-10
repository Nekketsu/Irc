using Irc.Client.Wpf.ViewModels;
using Irc.Client.Wpf.ViewModels.Tabs.Messages;
using Messages.Replies.CommandResponses;

namespace Irc.Client.Wpf.MessageHandlers.Replies.CommandResponses
{
    public class EndOfMotdReplyMessageHandler : IMessageHandler<EndOfMotdReply>
    {
        private readonly IrcViewModel viewModel;

        public EndOfMotdReplyMessageHandler(IrcViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public Task HandleAsync(EndOfMotdReply message)
        {
            var messageViewModel = new MessageViewModel(message.ToString());
            viewModel.DrawMessage(viewModel.Status, new MessageViewModel("-"));
            viewModel.DrawMessage(viewModel.Status, messageViewModel);

            return Task.CompletedTask;
        }
    }
}
