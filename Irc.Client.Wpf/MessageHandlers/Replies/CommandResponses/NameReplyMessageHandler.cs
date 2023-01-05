using Irc.Client.Wpf.ViewModels;
using Irc.Client.Wpf.ViewModels.Tabs;
using Messages.Replies.CommandResponses;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Irc.Client.Wpf.MessageHandlers.Replies.CommandResponses
{
    public class NameReplyMessageHandler : IMessageHandler<NameReply>
    {
        private readonly IrcViewModel viewModel;

        public NameReplyMessageHandler(IrcViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public Task HandleAsync(NameReply message)
        {
            viewModel.Irc.Join(message.ChannelName, message.Nicknames);

            var channel = viewModel.Chats
                .OfType<ChannelViewModel>()
                .Single(c => c.Target.Equals(message.ChannelName, StringComparison.InvariantCultureIgnoreCase));

            channel.Users = new ObservableCollection<string>(viewModel.Irc.GetUserByChannelName(message.ChannelName));

            return Task.CompletedTask;
        }
    }
}
