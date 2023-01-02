﻿using Irc.Client.Wpf.ViewModels;
using Irc.Client.Wpf.ViewModels.Tabs;
using Irc.Client.Wpf.ViewModels.Tabs.Messages;
using Irc.Messages.Messages;
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
                if (viewModel.Irc.UserIsInChannel(viewModel.Nickname, message.ChannelName))
                {
                    return Task.CompletedTask;
                }

                var messageViewModel = new MessageViewModel($"Now talking in {message.ChannelName}");
                var channel = (ChannelViewModel)viewModel.DrawMessage(message.ChannelName, messageViewModel);
                viewModel.FocusChat(channel);

                viewModel.Irc.Join(message.ChannelName, viewModel.Nickname);

                channel.Users = new(viewModel.Irc.Channels[message.ChannelName].Users.Keys);
            }
            else
            {
                var from = viewModel.Irc.GetNickName(message.From);
                var messageViewModel = new MessageViewModel($"{from} has joined {message.ChannelName}");
                var channel = (ChannelViewModel)viewModel.DrawMessage(message.ChannelName, messageViewModel);

                viewModel.Irc.Join(message.ChannelName, from);

                channel.Users = new(viewModel.Irc.Channels[message.ChannelName].Users.Keys);
            }

            return Task.CompletedTask;
        }
    }
}
