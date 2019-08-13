using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    public class WelcomeReply : IMessage
    {
        const string RPL_WELCOME = "001";
        public string Message { get; private set; }

        public WelcomeReply(string message)
        {
            Message = message;
        }

        public override string ToString()
        {
            return $"{RPL_WELCOME} {Message}";
        }
    }
}