using Irc.Client.Wpf.ViewModels;
using Irc.Client.Wpf.ViewModels.Tabs;
using Irc.Client.Wpf.ViewModels.Tabs.Messages;
using Irc.Messages.Messages;
using System;
using System.Threading.Tasks;

namespace Irc.Client.Wpf.MessageHandlers.Messages
{
    public class JoinMessageHandler : IMessageHandler<JoinMessage>
    {
        private readonly IrcViewModel viewModel;

        public JoinMessageHandler(IrcViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public Task HandleAsync(JoinMessage message)
        {
            if (message.From is null)
            {
                return Task.CompletedTask;
            }

            var from = viewModel.Irc.GetNickName(message.From);
            if (from.Equals(viewModel.Nickname, StringComparison.InvariantCultureIgnoreCase))
            {
                if (viewModel.Irc.UserIsInChannel(viewModel.Nickname, message.ChannelName))
                {
                    return Task.CompletedTask;
                }

                var messageViewModel = new MessageViewModel($"* Now talking in {message.ChannelName}") { MessageKind = MessageKind.Join };
                var channel = (ChannelViewModel)viewModel.DrawMessage(message.ChannelName, messageViewModel);

                viewModel.Irc.Join(message.ChannelName, viewModel.Nickname);

                channel.Users = new(viewModel.Irc.GetUserByChannelName(message.ChannelName));

                viewModel.FocusChat(channel);
            }
            else
            {
                var messageViewModel = new MessageViewModel($"* {from} has joined {message.ChannelName}") { MessageKind = MessageKind.Join };
                var channel = (ChannelViewModel)viewModel.DrawMessage(message.ChannelName, messageViewModel);

                viewModel.Irc.Join(message.ChannelName, from);

                channel.Users = new(viewModel.Irc.GetUserByChannelName(message.ChannelName));
            }

            return Task.CompletedTask;
        }
    }
}
