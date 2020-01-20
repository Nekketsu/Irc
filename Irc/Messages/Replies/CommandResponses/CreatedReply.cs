using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    public class CreatedReply : Reply
    {
        const string RPL_CREATED = "003";
        public string Message { get; private set; }

        public CreatedReply(string sender, string target, string message) : base(sender, target, RPL_CREATED)
        {
            Message = message;
        }

        public override string InnerToString()
        {
            return $":{Message}";
        }
    }
}