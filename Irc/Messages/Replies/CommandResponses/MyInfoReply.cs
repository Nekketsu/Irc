using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    [Command(RPL_MYINFO)]
    public class MyInfoReply : Reply
    {
        const string RPL_MYINFO = "004";
        public string Message { get; private set; }

        public MyInfoReply(string sender, string target, string message) : base(sender, target, RPL_MYINFO)
        {
            Message = message;
        }

        public override string InnerToString()
        {
            return $":{Message}";
        }
    }
}