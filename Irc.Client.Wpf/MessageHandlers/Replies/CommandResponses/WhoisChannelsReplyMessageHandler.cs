using Irc.Client.Wpf.ViewModels;
using Irc.Client.Wpf.ViewModels.Tabs.Messages;
using Messages.Replies.CommandResponses;

namespace Irc.Client.Wpf.MessageHandlers.Replies.CommandResponses
{
    public class WhoisChannelsReplyMessageHandler : IMessageHandler<WhoisChannelsReply>
    {
        private readonly IrcViewModel viewModel;

        public WhoisChannelsReplyMessageHandler(IrcViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public Task HandleAsync(WhoisChannelsReply message)
        {
            var text = $"{message.Nickname} on {string.Join(" ", message.ChannelNames)}";

            var messageViewModel = new MessageViewModel(text);
            viewModel.DrawMessage(viewModel.Status, messageViewModel);

            return Task.CompletedTask;
        }
    }
}
