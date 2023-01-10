using Irc.Client.Wpf.ViewModels;
using Irc.Client.Wpf.ViewModels.Tabs;
using Irc.Client.Wpf.ViewModels.Tabs.Messages;
using Irc.Messages.Messages;

namespace Irc.Client.Wpf.MessageHandlers.Messages.SendingMessages
{
    public class NoticeMessageHandler : IMessageHandler<NoticeMessage>
    {
        private readonly IrcViewModel viewModel;

        public NoticeMessageHandler(IrcViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public Task HandleAsync(NoticeMessage message)
        {
            if (message.From is null)
            {
                var text = $"-> -{message.Target}- {message.Text}";
                var messageViewModel = new MessageViewModel(text) { MessageKind = MessageKind.Notice };
                viewModel.DrawMessage(messageViewModel);
            }
            else
            {
                User from = message.From;

                var channels = viewModel.Chats.OfType<ChannelViewModel>();
                if (Channel.IsChannel(message.Target))
                {
                    var text = $"-{from.Nickname}:{message.Target}- {message.Text}";
                    var messageViewModel = new MessageViewModel(text) { MessageKind = MessageKind.Notice };

                    var channel = channels.Single(channel => channel.Target.Equals(message.Target, StringComparison.InvariantCultureIgnoreCase));
                    viewModel.DrawMessage(channel, messageViewModel);
                }
                else
                {
                    var user = viewModel.IrcClient.Users[from.Nickname];
                    if (user is not null)
                    {
                        var text = $"-{from.Nickname}- {message.Text}";
                        var messageViewModel = new MessageViewModel(text) { MessageKind = MessageKind.Notice };

                        foreach (var channel in channels)
                        {
                            if (user.Channels.Contains(channel.Target))
                            {
                                viewModel.DrawMessage(channel, messageViewModel);
                            }
                        }
                    }
                    else
                    {
                        var text = $"-{from.Nickname}- {message.Text}";
                        var messageViewModel = new MessageViewModel(text) { MessageKind = MessageKind.Notice };
                        viewModel.DrawMessage(viewModel.Status, messageViewModel);
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
