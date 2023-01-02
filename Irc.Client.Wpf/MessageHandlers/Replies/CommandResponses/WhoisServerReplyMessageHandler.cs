using Irc.Client.Wpf.ViewModels;
using Irc.Client.Wpf.ViewModels.Tabs.Messages;
using Messages.Replies.CommandResponses;
using System.Threading.Tasks;

namespace Irc.Client.Wpf.MessageHandlers.Replies.CommandResponses
{
    public class WhoisServerReplyMessageHandler : IMessageHandler<WhoisServerReply>
    {
        private readonly IrcViewModel viewModel;

        public WhoisServerReplyMessageHandler(IrcViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public Task HandleAsync(WhoisServerReply message)
        {
            var text = $"{message.Nickname} using {message.Server} {message.ServerInfo}";

            var messageViewModel = new MessageViewModel(text);
            viewModel.DrawMessage(viewModel.Status, messageViewModel);

            return Task.CompletedTask;
        }
    }
}
