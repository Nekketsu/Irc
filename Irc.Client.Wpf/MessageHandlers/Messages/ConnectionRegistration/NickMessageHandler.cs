using Irc.Client.Wpf.ViewModels;
using Irc.Client.Wpf.ViewModels.Tabs;
using Irc.Client.Wpf.ViewModels.Tabs.Messages;
using Irc.Messages.Messages;
using System;
using System.Linq;
using System.Threading.Tasks;

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
            if (message.From is null)
            {
                var messageViewModel = new MessageViewModel($"Ýour nick is now {message.Nickname}");
                viewModel.DrawMessage(viewModel.Status, messageViewModel);
            }

            if (message.From is not null)
            {
                var previousNickname = viewModel.Irc.GetNickName(message.From);
                viewModel.Irc.RenameUser(previousNickname, message.Nickname);

                var channels = viewModel.Irc.Channels.Values
                    .Where(channel => channel.Users.ContainsKey(message.Nickname))
                    .ToDictionary(channel => channel.Name);

                var chatViewModel = viewModel.Chats
                    .OfType<ChatViewModel>()
                    .SingleOrDefault(chat => chat.Target.Equals(previousNickname, StringComparison.InvariantCultureIgnoreCase));

                if (chatViewModel is not null)
                {
                    chatViewModel.Target = message.Nickname;
                }

                foreach (var channelViewModel in viewModel.Chats.OfType<ChannelViewModel>())
                {
                    if (channels.TryGetValue(channelViewModel.Target, out var channel))
                    {
                        channelViewModel.Users = new(channel.Users.Keys);

                        var messageViewModel = new MessageViewModel($"* {previousNickname} is now known as {message.Nickname}");
                        viewModel.DrawMessage(channelViewModel, messageViewModel);
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
