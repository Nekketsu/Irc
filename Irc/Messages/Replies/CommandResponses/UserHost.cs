using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    public class UserHostReply : IMessage
    {
        const string RPL_USERHOST = "003";
        public string Message { get; private set; }

        public UserHostReply(string message)
        {
            Message = message;
        }

        public override string ToString()
        {
            return $"{RPL_USERHOST} {Message}";
        }
    }
}