using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    public class EndOfWhoisReply : Reply
    {
        public const string DefaultMessage = "End of WHOIS list";

        const string RPL_ENDOFWHOIS = "318";
        public string Nickname { get; set; }
        public string Message { get; set; }

        public EndOfWhoisReply(string sender, string target, string nickname, string message) : base(sender, target, RPL_ENDOFWHOIS)
        {
            Nickname = nickname;
            Message = message;
        }

        public override string InnerToString()
        {
            return $"{Nickname} :{Message}";
        }
    }
}