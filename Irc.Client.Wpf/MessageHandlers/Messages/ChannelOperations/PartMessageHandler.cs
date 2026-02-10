using Irc.Client.Wpf.ViewModels;
using Irc.Client.Wpf.ViewModels.Tabs;
using Irc.Client.Wpf.ViewModels.Tabs.Messages;
using Irc.Messages.Messages;
using System.Collections.ObjectModel;

namespace Irc.Client.Wpf.MessageHandlers.Messages;

public class PartMessageHandler : IMessageHandler<PartMessage>
{
    private readonly IrcViewModel viewModel;

    public PartMessageHandler(IrcViewModel viewModel)
    {
        this.viewModel = viewModel;
    }

    public Task HandleAsync(PartMessage message)
    {
        var from = (User)message.From;
        if (from is null || from.Nickname == viewModel.Nickname)
        {
            return Task.CompletedTask;
        }

        var messageViewModel = new MessageViewModel($"* {from.Nickname} has left {message.ChannelName}") { MessageKind = MessageKind.Part };
        viewModel.DrawMessage(message.ChannelName, messageViewModel);

        var channel = viewModel.Chats.OfType<ChannelViewModel>().Single(c => c.Target == message.ChannelName);
        channel.Users = new ObservableCollection<string>(viewModel.IrcClient.Channels[message.ChannelName].Users.Select(u => (string)u.Nickname));

        return Task.CompletedTask;
    }
}
