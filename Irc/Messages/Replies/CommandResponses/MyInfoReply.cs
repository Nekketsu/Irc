using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    public class MyInfoReply : IMessage
    {
        const string RPL_MYINFO = "004";
        public string Message { get; private set; }

        public MyInfoReply(string message)
        {
            Message = message;
        }

        public override string ToString()
        {
            return $"{RPL_MYINFO} {Message}";
        }
    }
}