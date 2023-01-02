using Irc.Client.Wpf.ViewModels;
using Irc.Client.Wpf.ViewModels.Tabs.Messages;
using Messages.Replies.CommandResponses;
using System.Threading.Tasks;

namespace Irc.Client.Wpf.MessageHandlers.Replies.CommandResponses
{
    public class EndOfWhoisReplyMessageHandler : IMessageHandler<EndOfWhoisReply>
    {
        private readonly IrcViewModel viewModel;

        public EndOfWhoisReplyMessageHandler(IrcViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public Task HandleAsync(EndOfWhoisReply message)
        {
            var text = $"{message.Nickname} {message.Message}";

            var messageViewModel = new MessageViewModel(text);
            viewModel.DrawMessage(viewModel.Status, messageViewModel);
            viewModel.DrawMessage(viewModel.Status, new MessageViewModel("-"));

            return Task.CompletedTask;
        }
    }
}
