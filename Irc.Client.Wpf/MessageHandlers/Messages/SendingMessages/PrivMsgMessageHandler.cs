using Irc.Client.Wpf.ViewModels;
using Irc.Client.Wpf.ViewModels.Tabs.Messages;
using Irc.Messages.Messages;

namespace Irc.Client.Wpf.MessageHandlers.Messages;

public class PrivMsgMessageHandler : IMessageHandler<PrivMsgMessage>
{
    private readonly IrcViewModel viewModel;

    public PrivMsgMessageHandler(IrcViewModel viewModel)
    {
        this.viewModel = viewModel;
    }

    public Task HandleAsync(PrivMsgMessage message)
    {
        if (message.From is null)
        {
            var from = viewModel.Nickname;
            var target = message.Target;

            var messageViewModel = new ChatMessageViewModel(from, message.Text);
            viewModel.DrawMessage(target, messageViewModel);
        }
        else
        {
            User from = message.From;
            var target = message.Target.Equals(viewModel.Nickname, StringComparison.InvariantCultureIgnoreCase)
                ? (string)from.Nickname
                : message.Target;

            var messageViewModel = new ChatMessageViewModel((string)from.Nickname, message.Text);
            viewModel.DrawMessage(target, messageViewModel);
        }

        return Task.CompletedTask;
    }
}
