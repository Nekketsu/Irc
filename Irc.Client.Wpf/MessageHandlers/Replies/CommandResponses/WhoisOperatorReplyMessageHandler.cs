using Irc.Client.Wpf.ViewModels;
using Irc.Client.Wpf.ViewModels.Tabs.Messages;
using Irc.Messages.Replies;

namespace Irc.Client.Wpf.MessageHandlers.Replies.CommandResponses;

public class WhoisOperatorReplyMessageHandler : IMessageHandler<WhoisOperatorReply>
{
    private readonly IrcViewModel viewModel;

    public WhoisOperatorReplyMessageHandler(IrcViewModel viewModel)
    {
        this.viewModel = viewModel;
    }

    public Task HandleAsync(WhoisOperatorReply message)
    {
        var text = $"{message.Nickname} {message.Message}";

        var messageViewModel = new MessageViewModel(text);
        viewModel.DrawMessage(viewModel.Status, messageViewModel);

        return Task.CompletedTask;
    }
}
