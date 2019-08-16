using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    public class UserHostReply : Reply
    {
        const string RPL_USERHOST = "302";
        public string Message { get; private set; }

        public UserHostReply(string target, string message) : base(target, RPL_USERHOST)
        {
            Message = message;
        }

        public override string InnerToString()
        {
            return $":{Message}";
        }
    }
}