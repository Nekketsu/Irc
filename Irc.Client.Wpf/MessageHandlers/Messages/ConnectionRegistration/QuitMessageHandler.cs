using Irc.Client.Wpf.ViewModels;
using Irc.Client.Wpf.ViewModels.Tabs;
using Irc.Client.Wpf.ViewModels.Tabs.Messages;
using Irc.Messages.Messages;

namespace Irc.Client.Wpf.MessageHandlers.Messages
{
    public class QuitMessageHandler : IMessageHandler<QuitMessage>
    {
        private readonly IrcViewModel viewModel;

        public QuitMessageHandler(IrcViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public Task HandleAsync(QuitMessage message)
        {
            if (message.Target is not null)
            {
                var target = (User)message.Target;
                var text = $"* {target.Nickname} has quit IRC ({message.Reason})";
                var messageViewModel = new MessageViewModel(text) { MessageKind = MessageKind.Quit };

                foreach (var channel in viewModel.Chats.OfType<ChannelViewModel>().ToArray())
                {
                    if (viewModel.IrcClient.Channels[channel.Target].Users.Contains(target.Nickname))
                    {
                        viewModel.DrawMessage(channel, messageViewModel);
                        channel.Users = new(viewModel.IrcClient.Channels[channel.Target].Users.Select(u => (string)u.Nickname));
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
