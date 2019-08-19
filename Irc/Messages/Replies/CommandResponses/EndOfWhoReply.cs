using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    public class EndOfWhoReply : Reply
    {
        public const string DefaultMessage = "End of WHO list";
        
        const string RPL_ENDOFWHO = "315";
        public string Mask { get; set; }
        public string Message { get; set; }

        public EndOfWhoReply(string target, string mask, string message) : base(target, RPL_ENDOFWHO)
        {
            Mask = mask;
            Message = message;
        }

        public override string InnerToString()
        {
            return $"{Mask} :{Message}";
        }
    }
}