using Irc.Client.Wpf.ViewModels;
using Irc.Client.Wpf.ViewModels.Tabs;
using Irc.Client.Wpf.ViewModels.Tabs.Messages;
using Messages.Replies.CommandResponses;
using System.Collections.ObjectModel;
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
            var messageViewModel = new MessageViewModel(message.ToString());
            var channel = (ChannelViewModel)viewModel.DrawMessage(message.ChannelName, messageViewModel);
            viewModel.Irc.Join(message.ChannelName, message.Nicknames);

            channel.Users = new ObservableCollection<string>(viewModel.Irc.Channels[message.ChannelName].Users.Keys);

            return Task.CompletedTask;
        }
    }
}
