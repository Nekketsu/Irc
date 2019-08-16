using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    public class MyInfoReply : Reply
    {
        const string RPL_MYINFO = "004";
        public string Message { get; private set; }

        public MyInfoReply(string target, string message) : base(target, RPL_MYINFO)
        {
            Message = message;
        }

        public override string InnerToString()
        {
            return $":{Message}";
        }
    }
}