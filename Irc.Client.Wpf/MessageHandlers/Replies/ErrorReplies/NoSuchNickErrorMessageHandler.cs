using Irc.Client.Wpf.ViewModels;
using Irc.Client.Wpf.ViewModels.Tabs.Messages;
using Messages.Replies.ErrorReplies;

namespace Irc.Client.Wpf.MessageHandlers.Replies.ErrorReplies
{
    public class NoSuchNickErrorMessageHandler : IMessageHandler<NoSuchNickError>
    {
        private readonly IrcViewModel viewModel;

        public NoSuchNickErrorMessageHandler(IrcViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public Task HandleAsync(NoSuchNickError message)
        {
            var text = $"{message.Nickname} {message.Message}";
            var messageViewModel = new MessageViewModel(text);

            viewModel.DrawMessage(viewModel.Status, messageViewModel);
            viewModel.DrawMessage(viewModel.Status, new MessageViewModel("-"));

            return Task.CompletedTask;
        }
    }
}
