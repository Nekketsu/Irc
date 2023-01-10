using Irc.Client.Wpf.ViewModels;
using Irc.Client.Wpf.ViewModels.Tabs.Messages;
using Messages.Replies.CommandResponses;

namespace Irc.Client.Wpf.MessageHandlers.Replies.CommandResponses
{
    public class TopicWhoTimeReplyMessageHandler : IMessageHandler<TopicWhoTimeReply>
    {
        private readonly IrcViewModel viewModel;

        public TopicWhoTimeReplyMessageHandler(IrcViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public Task HandleAsync(TopicWhoTimeReply message)
        {
            var text = $"* Set by {message.Nickname} on {message.SetAt}";
            var messageViewModel = new MessageViewModel(text) { MessageKind = MessageKind.Topic };
            viewModel.DrawMessage(message.ChannelName, messageViewModel);

            return Task.CompletedTask;
        }
    }
}
