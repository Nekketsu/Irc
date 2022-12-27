using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    [Command(RPL_WELCOME)]
    public class WelcomeReply : Reply
    {
        const string RPL_WELCOME = "001";
        public string Message { get; private set; }

        public WelcomeReply(string sender, string target, string message) : base(sender, target, RPL_WELCOME)
        {
            Message = message;
        }

        public override string InnerToString()
        {
            return $":{Message}";
        }
    }
}