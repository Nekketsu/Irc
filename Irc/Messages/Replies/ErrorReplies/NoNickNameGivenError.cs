using Irc.Messages;

namespace Messages.Replies.ErrorReplies
{
    public class NoNicknameGivenError : Reply
    {
        public const string DefaultMessage = "No nickname given";
        
        const string ERR_NONicknameGIVEN = "431";
        public string Message { get; private set; }

        public NoNicknameGivenError(string sender, string target, string message) : base(sender, target, ERR_NONicknameGIVEN)
        {
            Message = message;
        }

        public override string InnerToString()
        {
            return $":{Message}";
        }
    }
}