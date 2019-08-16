using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    public class WelcomeReply : Reply
    {
        const string RPL_WELCOME = "001";
        public string Message { get; private set; }

        public WelcomeReply(string target, string message) : base(target, RPL_WELCOME)
        {
            Message = message;
        }

        public override string InnerToString()
        {
            return $":{Message}";
        }
    }
}