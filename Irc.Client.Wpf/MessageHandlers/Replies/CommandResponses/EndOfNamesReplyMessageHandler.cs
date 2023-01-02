using Irc.Client.Wpf.ViewModels;
using Irc.Client.Wpf.ViewModels.Tabs.Messages;
using Messages.Replies.CommandResponses;
using System.Threading.Tasks;

namespace Irc.Client.Wpf.MessageHandlers.Replies.CommandResponses
{
    public class EndOfNamesReplyMessageHandler : IMessageHandler<EndOfNamesReply>
    {
        private readonly IrcViewModel viewModel;

        public EndOfNamesReplyMessageHandler(IrcViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public Task HandleAsync(EndOfNamesReply message)
        {
            var messageViewModel = new MessageViewModel(message.ToString());
            viewModel.DrawMessage(message.ChannelName, messageViewModel);

            return Task.CompletedTask;
        }
    }
}
