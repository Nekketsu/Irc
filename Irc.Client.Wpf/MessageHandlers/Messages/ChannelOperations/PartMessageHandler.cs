using Irc.Client.Wpf.ViewModels;
using Irc.Client.Wpf.ViewModels.Tabs;
using Irc.Client.Wpf.ViewModels.Tabs.Messages;
using Irc.Messages.Messages;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Irc.Client.Wpf.MessageHandlers.Messages
{
    public class PartMessageHandler : IMessageHandler<PartMessage>
    {
        private readonly IrcViewModel viewModel;

        public PartMessageHandler(IrcViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public Task HandleAsync(PartMessage message)
        {
            if (message.From is null)
            {
                viewModel.Irc.Part(message.ChannelName, viewModel.Nickname);
            }
            else
            {
                var from = viewModel.Irc.GetNickName(message.From);
                if (from.Equals(viewModel.Nickname, StringComparison.InvariantCultureIgnoreCase))
                {
                    return Task.CompletedTask;
                }

                var messageViewModel = new MessageViewModel($"* {from} has left {message.ChannelName}") { MessageKind = MessageKind.Part };
                viewModel.DrawMessage(message.ChannelName, messageViewModel);

                viewModel.Irc.Part(message.ChannelName, from);

                var channel = viewModel.Chats.OfType<ChannelViewModel>().Single(c => c.Target == message.ChannelName);
                channel.Users = new ObservableCollection<string>(viewModel.Irc.Channels[message.ChannelName].Users.Keys);
            }

            return Task.CompletedTask;
        }
    }
}
