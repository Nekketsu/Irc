using Irc.Client.Wpf.ViewModels;
using Irc.Client.Wpf.ViewModels.Tabs.Messages;
using Messages.Replies.CommandResponses;
using System.Threading.Tasks;

namespace Irc.Client.Wpf.MessageHandlers.Replies.CommandResponses
{
    public class TopicReplyMessageHandler : IMessageHandler<TopicReply>
    {
        private readonly IrcViewModel viewModel;

        public TopicReplyMessageHandler(IrcViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public Task HandleAsync(TopicReply message)
        {
            var messageViewModel = new MessageViewModel(message.ToString());
            viewModel.DrawMessage(message.ChannelName, messageViewModel);

            return Task.CompletedTask;
        }
    }
}
