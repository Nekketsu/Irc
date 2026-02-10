using Irc.Client.Wpf.ViewModels;
using Irc.Client.Wpf.ViewModels.Tabs.Messages;
using Irc.Messages.Replies.CommandResponses;

namespace Irc.Client.Wpf.MessageHandlers.Replies.CommandResponses;

public class WhoisModesReplyMessageHandler : IMessageHandler<WhoisModesReply>
{
    private readonly IrcViewModel viewModel;

    public WhoisModesReplyMessageHandler(IrcViewModel viewModel)
    {
        this.viewModel = viewModel;
    }

    public Task HandleAsync(WhoisModesReply message)
    {
        var text = $"{message.Nickname} {message.Text}";

        var messageViewModel = new MessageViewModel(text);
        viewModel.DrawMessage(viewModel.Status, messageViewModel);

        return Task.CompletedTask;
    }
}
