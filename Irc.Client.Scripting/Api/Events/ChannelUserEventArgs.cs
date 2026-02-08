namespace Irc.Client.Scripting.Api.Events
{
    /// <summary>
    /// Event arguments for user joined/left channel events.
    /// Contains read-only information about the user and channel.
    /// </summary>
    public class ChannelUserEventArgs
    {
        /// <summary>
        /// Information about the channel.
        /// </summary>
        public ChannelInfo Channel { get; }

        /// <summary>
        /// Information about the user.
        /// </summary>
        public UserInfo User { get; }

        /// <summary>
        /// Optional reason (for leave/quit events).
        /// </summary>
        public string? Reason { get; }

        public ChannelUserEventArgs(ChannelInfo channel, UserInfo user, string? reason = null)
        {
            Channel = channel;
            User = user;
            Reason = reason;
        }

        internal static ChannelUserEventArgs FromEvent(Channel channel, Nickname nickname, string? reason = null)
        {
            return new ChannelUserEventArgs(
                ChannelInfo.FromChannel(channel),
                UserInfo.FromNickname(nickname),
                reason
            );
        }
    }
}
