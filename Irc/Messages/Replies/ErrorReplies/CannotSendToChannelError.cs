using Irc.Messages;

namespace Messages.Replies.ErrorReplies
{
    public class CannotSendToChannelError : Reply
    {
        public const string ERR_CANNOTSENDTOCHAN = "404";
        public string ChannelName { get; set; }
        public string Message { get; set; }

        public CannotSendToChannelError(string sender, string target, string channelName, string message) : base(sender, target, ERR_CANNOTSENDTOCHAN)
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