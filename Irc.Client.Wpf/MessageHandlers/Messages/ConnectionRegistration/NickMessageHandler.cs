using Irc.Client.Wpf.ViewModels;
using Irc.Client.Wpf.ViewModels.Tabs;
using Irc.Client.Wpf.ViewModels.Tabs.Messages;
using Irc.Messages.Messages;

namespace Irc.Client.Wpf.MessageHandlers.Messages.ConnectionRegistration
{
    public class NickMessageHandler : IMessageHandler<NickMessage>
    {
        private readonly IrcViewModel viewModel;

        public NickMessageHandler(IrcViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public Task HandleAsync(NickMessage message)
        {
            User from = message.From;

            if (from is null)
            {
                var messageViewModel = new MessageViewModel($"* Your nick is now {message.Nickname}") { MessageKind = MessageKind.Nick };
                viewModel.DrawMessage(viewModel.Status, messageViewModel);
            }

            if (from is not null)
            {
                var channels = viewModel.IrcClient.Channels
                    .Where(channel => channel.Users.Contains(message.Nickname))
                    .ToDictionary(channel => channel.Name);

                var chatViewModel = viewModel.Chats
                    .OfType<ChatViewModel>()
                    .SingleOrDefault(chat => chat.Target.Equals((string)from.Nickname, StringComparison.InvariantCultureIgnoreCase));

                if (chatViewModel is not null)
                {
                    chatViewModel.Target = message.Nickname;
                }

                foreach (var channelViewModel in viewModel.Chats.OfType<ChannelViewModel>())
                {
                    if (channels.TryGetValue(channelViewModel.Target, out var channel))
                    {
                        channelViewModel.Users = new(channel.Users.Select(u => (string)u.Nickname));

                        var messageViewModel = new MessageViewModel($"* {from.Nickname} is now known as {message.Nickname}") { MessageKind = MessageKind.Nick };
                        viewModel.DrawMessage(channelViewModel, messageViewModel);
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
