using Irc.Messages;

namespace Messages.Replies.ErrorReplies
{
    public class CannotSendToChannelError : Reply
    {
        public const string ERR_CANNOTSENDTOCHAN = "404";
        public string ChannelName { get; set; }
        public string Message { get; set; }

        public CannotSendToChannelError(string target, string channelName, string message) : base(target, ERR_CANNOTSENDTOCHAN)
        {
            Message = message;
        }

        public override string InnerToString()
        {
            return $"{ChannelName} :{Message}";
        }
    }
}