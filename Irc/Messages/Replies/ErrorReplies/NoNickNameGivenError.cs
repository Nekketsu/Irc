using Irc.Messages;

namespace Messages.Replies.ErrorReplies;

[Command(ERR_NONICKNAMEGIVEN)]
public class NoNicknameGivenError : Reply
{
    public const string DefaultMessage = "No nickname given";

    const string ERR_NONICKNAMEGIVEN = "431";
    public string Message { get; private set; }

    public NoNicknameGivenError(string sender, string target, string message) : base(sender, target, ERR_NONICKNAMEGIVEN)
    {
        Message = message;
    }

    public override string InnerToString()
    {
        return $":{Message}";
    }
}