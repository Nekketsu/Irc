using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    public class YourHostReply : IMessage
    {
        const string RPL_YOURHOST = "002";
        public string Message { get; set; }

        public YourHostReply(string message)
        {
            Message = message;
        }

        public override string ToString()
        {
            return $"{RPL_YOURHOST} {Message}";
        }
    }
}