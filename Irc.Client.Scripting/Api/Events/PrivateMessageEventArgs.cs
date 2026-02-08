namespace Irc.Client.Scripting.Api.Events
{
    /// <summary>
    /// Event arguments for private message events.
    /// Contains read-only information about the message and sender.
    /// </summary>
    public class PrivateMessageEventArgs
    {
        /// <summary>
        /// Information about the user who sent the message.
        /// </summary>
        public UserInfo Sender { get; }

        /// <summary>
        /// Content of the message.
        /// </summary>
        public string Message { get; }

        public PrivateMessageEventArgs(UserInfo sender, string message)
        {
            Sender = sender;
            Message = message;
        }

        internal static PrivateMessageEventArgs FromEvent(Nickname from, string message)
        {
            return new PrivateMessageEventArgs(
                UserInfo.FromNickname(from),
                message
            );
        }
    }
}
