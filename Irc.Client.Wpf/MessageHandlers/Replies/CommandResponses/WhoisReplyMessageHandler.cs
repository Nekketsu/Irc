using Irc.Client.Wpf.ViewModels;
using Irc.Client.Wpf.ViewModels.Tabs.Messages;
using Messages.Replies.CommandResponses;
using System.Threading.Tasks;

namespace Irc.Client.Wpf.MessageHandlers.Replies.CommandResponses
{
    public class WhoisReplyMessageHandler : IMessageHandler<WhoisReply>
    {
        private readonly IrcViewModel viewModel;

        public WhoisReplyMessageHandler(IrcViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public Task HandleAsync(WhoisReply message)
        {
            var text = $"{message.Nickname} is {message.User}@{message.Host} * {message.RealName}";

            var messageViewModel = new MessageViewModel(text);
            viewModel.DrawMessage(viewModel.Status, new MessageViewModel("-"));
            viewModel.DrawMessage(viewModel.Status, messageViewModel);

            return Task.CompletedTask;
        }
    }
}
