using Irc.Client.Wpf.ViewModels;
using Irc.Client.Wpf.ViewModels.Tabs;
using Irc.Client.Wpf.ViewModels.Tabs.Messages;
using Irc.Messages.Messages;

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

            User from = message.From;
            if (from.Nickname == viewModel.Nickname)
            {
                var messageViewModel = new MessageViewModel($"* Now talking in {message.ChannelName}") { MessageKind = MessageKind.Join };
                var channel = (ChannelViewModel)viewModel.DrawMessage(message.ChannelName, messageViewModel);

                channel.Users = new(viewModel.IrcClient.Channels[message.ChannelName].Users.Select(u => (string)u.Nickname));

                viewModel.FocusChat(channel);
            }
            else
            {
                var messageViewModel = new MessageViewModel($"* {from.Nickname} has joined {message.ChannelName}") { MessageKind = MessageKind.Join };
                var channel = (ChannelViewModel)viewModel.DrawMessage(message.ChannelName, messageViewModel);


                channel.Users = new(viewModel.IrcClient.Channels[message.ChannelName].Users.Select(u => (string)u.Nickname));
            }

            return Task.CompletedTask;
        }
    }
}
