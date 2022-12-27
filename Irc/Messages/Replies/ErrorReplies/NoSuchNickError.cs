using Irc.Messages;

namespace Messages.Replies.ErrorReplies
{
    [Command(ERR_NOSUCHNICK)]
    public class NoSuchNickError : Reply
    {
        public const string DefaultMessage = "No such nick";

        const string ERR_NOSUCHNICK = "401";
        public string Nickname { get; set; }
        public string Message { get; private set; }

        public NoSuchNickError(string sender, string target, string nickname, string message) : base(sender, target, ERR_NOSUCHNICK)
        {
            Nickname = nickname;
            Message = message;
        }

        public override string InnerToString()
        {
            return $"{Nickname} :{Message}";
        }
    }
}