using Irc.Client.Wpf.ViewModels;
using Irc.Client.Wpf.ViewModels.Tabs;
using Irc.Client.Wpf.ViewModels.Tabs.Messages;
using Irc.Messages.Messages;
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
                var text = $"* {target} has quit IRC ({message.Reason})";
                var messageViewModel = new MessageViewModel(text) { MessageKind = MessageKind.Quit };

                viewModel.Irc.Quit(target);

                foreach (var channel in viewModel.Chats.OfType<ChannelViewModel>().ToArray())
                {
                    if (viewModel.Irc.Channels[channel.Target].Users.ContainsKey(target))
                    {
                        viewModel.DrawMessage(channel, messageViewModel);
                        channel.Users = new(viewModel.Irc.GetUserByChannelName(channel.Target));
                    }
                }
            }
            else
            {
                viewModel.Irc.Quit(viewModel.Nickname);
            }

            return Task.CompletedTask;
        }
    }
}
