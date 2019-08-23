using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    public class ListEndReply : Reply
    {
        public const string DefaultMessage = "End of LIST";
        
        const string RPL_LISTEND = "323";
        public string Message { get; set; }

        public ListEndReply(string target, string message) : base(target, RPL_LISTEND)
        {
            Message = message;
        }

        public override string InnerToString()
        {
            return $":{Message}";
        }
    }
}