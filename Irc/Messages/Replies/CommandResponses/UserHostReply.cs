using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    [Command(RPL_USERHOST)]
    public class UserHostReply : Reply
    {
        const string RPL_USERHOST = "302";
        public string Message { get; private set; }

        public UserHostReply(string sender, string target, string message) : base(sender, target, RPL_USERHOST)
        {
            Message = message;
        }

        public override string InnerToString()
        {
            return $":{Message}";
        }
    }
}