using Irc.Client.Wpf.ViewModels;
using Irc.Client.Wpf.ViewModels.Tabs;
using Irc.Client.Wpf.ViewModels.Tabs.Messages;
using Irc.Messages.Messages;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Irc.Client.Wpf.MessageHandlers.Messages
{
    public class QuiteMessageHandler : IMessageHandler<QuitMessage>
    {
        private readonly IrcViewModel viewModel;

        public QuiteMessageHandler(IrcViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public Task HandleAsync(QuitMessage message)
        {
            if (message.Target is not null)
            {
                var target = viewModel.Irc.GetNickName(message.Target);
                foreach (var chat in viewModel.Chats.OfType<ChatViewModel>())
                {
                    if (string.Equals(chat.Target, target, StringComparison.InvariantCultureIgnoreCase))
                    {
                        var messageViewModel = new MessageViewModel(message.Reason);
                        viewModel.DrawMessage(target, messageViewModel);
                    }
                }

                viewModel.Irc.Quit(target);
            }
            else
            {
                viewModel.Irc.Quit(viewModel.Nickname);
            }

            return Task.CompletedTask;
        }
    }
}
