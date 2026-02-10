namespace Irc.Client.Scripting.Api.Events;

/// <summary>
/// Event arguments for channel message events.
/// Contains read-only information about the message, sender, and channel.
/// </summary>
public class ChannelMessageEventArgs
{
    /// <summary>
    /// Information about the channel where the message was sent.
    /// </summary>
    public ChannelInfo Channel { get; }

    /// <summary>
    /// Information about the user who sent the message.
    /// </summary>
    public UserInfo Sender { get; }

    /// <summary>
    /// Content of the message.
    /// </summary>
    public string Message { get; }

    public ChannelMessageEventArgs(ChannelInfo channel, UserInfo sender, string message)
    {
        Channel = channel;
        Sender = sender;
        Message = message;
    }

    internal static ChannelMessageEventArgs FromEvent(Channel channel, Nickname from, string message)
    {
        return new ChannelMessageEventArgs(
            ChannelInfo.FromChannel(channel),
            UserInfo.FromNickname(from),
            message
        );
    }
}
