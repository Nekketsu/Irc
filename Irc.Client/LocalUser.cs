using Irc.Client.Events;

namespace Irc.Client;

public class LocalUser : User
{
    public event EventHandler<MessageEventArgs> MessageReceived;
    public event EventHandler<MessageEventArgs> NoticeReceived;
    public event EventHandler<ChannelEventArgs> ChannelJoined;
    public event EventHandler<ChannelEventArgs> ChannelParted;

    internal void OnMessageReceived(Nickname nickname, string message)
    {
        MessageReceived?.Invoke(this, new(nickname, message));
    }

    internal void OnNoticeReceived(Nickname nickname, string message)
    {
        NoticeReceived?.Invoke(this, new(nickname, message));
    }

    internal void OnChannelJoined(Channel channel)
    {
        ChannelJoined?.Invoke(this, new ChannelEventArgs(channel));
    }

    internal void OnChannelParted(Channel channel)
    {
        ChannelParted?.Invoke(this, new ChannelEventArgs(channel));
    }
}
