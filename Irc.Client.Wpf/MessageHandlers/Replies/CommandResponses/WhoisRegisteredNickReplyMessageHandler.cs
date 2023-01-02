using Irc.Client.Wpf.ViewModels;
using Irc.Client.Wpf.ViewModels.Tabs.Messages;
using Irc.Messages.Replies.CommandResponses;
using System.Threading.Tasks;

namespace Irc.Client.Wpf.MessageHandlers.Replies.CommandResponses
{
    public class WhoisRegisteredNickReplyMessageHandler : IMessageHandler<WhoisRegisteredNickReply>
    {
        private readonly IrcViewModel viewModel;

        public WhoisRegisteredNickReplyMessageHandler(IrcViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public Task HandleAsync(WhoisRegisteredNickReply message)
        {
            var text = $"{message.Nickname} {message.Text}";

            var messageViewModel = new MessageViewModel(text);
            viewModel.DrawMessage(viewModel.Status, messageViewModel);

            return Task.CompletedTask;
        }
    }
}
