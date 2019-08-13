using Irc.Messages;

namespace Messages.Replies.ErrorReplies
{
    public class NoRecipientError : IMessage
    {
        public const string ERR_NORECIPIENT = "401";
        public string Message { get; set; }

        public NoRecipientError(string message)
        {
            Message = message;
        }

        public override string ToString()
        {
            return $"{ERR_NORECIPIENT} {Message}";
        }
    }
}