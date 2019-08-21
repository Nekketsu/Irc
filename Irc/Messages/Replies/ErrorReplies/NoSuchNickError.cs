using Irc.Messages;

namespace Messages.Replies.ErrorReplies
{
    public class NoSuchNickError : Reply
    {
        public const string DefaultMessage = "No such nick";
        
        const string ERR_NOSUCHNICK = "401";
        public string Nickname { get; set; }
        public string Message { get; private set; }

        public NoSuchNickError(string target, string nickname, string message) : base(target, ERR_NOSUCHNICK)
        {
            Message = message;
        }

        public override string InnerToString()
        {
            return $"{Nickname} :{Message}";
        }
    }
}