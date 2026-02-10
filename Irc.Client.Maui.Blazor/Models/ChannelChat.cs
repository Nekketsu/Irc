using Microsoft.JSInterop;

namespace Irc.Client.Maui.Blazor.Models;

public class ChannelChat : Chat
{
    public UserCollection Users { get; }

    public ChannelChat(Pages.Index index, Channel channel) : base(index)
    {
        Users = new();

        Id = $"Channel_{channel.Name}";
        Name = channel.Name;

        channel.MessageReceived += Channel_MessageReceived;
        channel.UserJoined += Channel_UserJoined;
        Users = channel.Users;
    }

    private async void Channel_UserJoined(object sender, Events.UserEventArgs e)
    {
        if (index.CurrentChat == this)
        {
            index.OnStateHasChanged();
        }

        await index.JSRuntime.InvokeVoidAsync("scrollToBottom", Id);
    }

    private async void Channel_MessageReceived(object sender, Events.MessageEventArgs e)
    {
        await Speak(e.From, e.Message);
    }
}
