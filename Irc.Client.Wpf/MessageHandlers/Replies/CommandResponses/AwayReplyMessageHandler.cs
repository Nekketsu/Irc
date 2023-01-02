using Irc.Client.Wpf.ViewModels.Tabs.Messages;
using Irc.Client.Wpf.ViewModels;
using Irc.Messages.Replies.CommandResponses;
using System.Threading.Tasks;

namespace Irc.Client.Wpf.MessageHandlers.Replies.CommandResponses
{
    public class AwayReplyMessageHandler : IMessageHandler<AwayReply>
    {
        private readonly IrcViewModel viewModel;

        public AwayReplyMessageHandler(IrcViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public Task HandleAsync(AwayReply message)
        {
            var text = $"{message.Nickname} is away: {message.Text}";

            var messageViewModel = new MessageViewModel(text);
            viewModel.DrawMessage(viewModel.Status, messageViewModel);

            return Task.CompletedTask;
        }
    }
}
