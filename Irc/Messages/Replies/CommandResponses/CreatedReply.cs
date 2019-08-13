using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    public class CreatedReply : IMessage
    {
        const string RPL_CREATED = "003";
        public string Message { get; private set; }

        public CreatedReply(string message)
        {
            Message = message;
        }

        public override string ToString()
        {
            return $"{RPL_CREATED} {Message}";
        }
    }
}