using Irc.Client.Wpf.ViewModels;
using Irc.Client.Wpf.ViewModels.Tabs;
using Messages.Replies.CommandResponses;
using System.Collections.ObjectModel;

namespace Irc.Client.Wpf.MessageHandlers.Replies.CommandResponses;

public class NameReplyMessageHandler : IMessageHandler<NameReply>
{
    private readonly IrcViewModel viewModel;

    public NameReplyMessageHandler(IrcViewModel viewModel)
    {
        this.viewModel = viewModel;
    }

    public Task HandleAsync(NameReply message)
    {
        var channel = viewModel.Chats
            .OfType<ChannelViewModel>()
            .Single(c => c.Target.Equals(message.ChannelName, StringComparison.InvariantCultureIgnoreCase));

        var users = viewModel.IrcClient.Channels[message.ChannelName].Users.users.Values.ToArray();

        channel.Users = new ObservableCollection<string>(viewModel.IrcClient.Channels[message.ChannelName].Users.Select(u => (string)u.Nickname));

        return Task.CompletedTask;
    }
}
