using Irc.Client.Wpf.ViewModels;
using Irc.Client.Wpf.ViewModels.Tabs;
using Irc.Client.Wpf.ViewModels.Tabs.Messages;
using Irc.Messages.Messages;
using System;
using System.Linq;
using System.Threading.Tasks;

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
                var from = viewModel.Irc.GetNickName(message.From);

                var channels = viewModel.Chats.OfType<ChannelViewModel>();
                if (viewModel.Irc.IsChannel(message.Target))
                {
                    var text = $"-{from}:{message.Target}- {message.Text}";
                    var messageViewModel = new MessageViewModel(text) { MessageKind = MessageKind.Notice };

                    var channel = channels.Single(channel => channel.Target.Equals(message.Target, StringComparison.InvariantCultureIgnoreCase));
                    viewModel.DrawMessage(channel, messageViewModel);
                }
                else if (viewModel.Irc.Users.TryGetValue(from, out var user))
                {
                    var text = $"-{from}- {message.Text}";
                    var messageViewModel = new MessageViewModel(text) { MessageKind = MessageKind.Notice };

                    foreach (var channel in channels)
                    {
                        if (user.Channels.ContainsKey(channel.Target))
                        {
                            viewModel.DrawMessage(channel, messageViewModel);
                        }
                    }
                }
                else
                {
                    var text = $"-{from}- {message.Text}";
                    var messageViewModel = new MessageViewModel(text) { MessageKind = MessageKind.Notice };
                    viewModel.DrawMessage(viewModel.Status, messageViewModel);
                }
            }

            return Task.CompletedTask;
        }
    }
}
