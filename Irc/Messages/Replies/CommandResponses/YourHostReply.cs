using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    public class YourHostReply : Reply
    {
        const string RPL_YOURHOST = "002";
        public string Message { get; set; }

        public YourHostReply(string target, string message) : base(target, RPL_YOURHOST)
        {
            Message = message;
        }

        public override string InnerToString()
        {
            return $":{Message}";
        }
    }
}