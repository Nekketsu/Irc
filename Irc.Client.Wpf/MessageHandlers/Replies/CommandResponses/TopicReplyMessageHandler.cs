using Irc.Client.Wpf.ViewModels;
using Irc.Client.Wpf.ViewModels.Tabs;
using Irc.Client.Wpf.ViewModels.Tabs.Messages;
using Messages.Replies.CommandResponses;

namespace Irc.Client.Wpf.MessageHandlers.Replies.CommandResponses;

public class TopicReplyMessageHandler : IMessageHandler<TopicReply>
{
    private readonly IrcViewModel viewModel;

    public TopicReplyMessageHandler(IrcViewModel viewModel)
    {
        this.viewModel = viewModel;
    }

    public Task HandleAsync(TopicReply message)
    {
        var text = $"* Topic is '{message.Topic}'";
        var messageViewModel = new MessageViewModel(text) { MessageKind = MessageKind.Topic };
        viewModel.DrawMessage(message.ChannelName, messageViewModel);

        if (viewModel.SelectedTab is ChannelViewModel channel && channel.Target.Equals(message.ChannelName, StringComparison.InvariantCultureIgnoreCase))
        {
            viewModel.UpdateTitle();
        }

        return Task.CompletedTask;
    }
}
