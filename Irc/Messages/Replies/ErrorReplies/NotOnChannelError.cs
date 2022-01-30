using Irc.Messages;

namespace Messages.Replies.ErrorReplies
{
    public class NotOnChannelError : Reply
    {
        public const string DefaultMessage = "You're not on that channel";

        const string ERR_NOTONCHANNEL = "442";
        public string ChannelName { get; set; }
        public string Message { get; private set; }

        public NotOnChannelError(string sender, string target, string channelName, string message) : base(sender, target, ERR_NOTONCHANNEL)
        {
            ChannelName = channelName;
            Message = message;
        }

        public override string InnerToString()
        {
            return $"{ChannelName} :{Message}";
        }
    }
}