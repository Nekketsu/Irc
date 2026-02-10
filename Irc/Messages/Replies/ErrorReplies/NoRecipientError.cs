using Irc.Messages;

namespace Messages.Replies.ErrorReplies;

[Command(ERR_NORECIPIENT)]
public class NoRecipientError : Reply
{
    public const string ERR_NORECIPIENT = "411";
    public string Message { get; set; }

    public NoRecipientError(string sender, string target, string message) : base(sender, target, ERR_NORECIPIENT)
    {
        Message = message;
    }

    public override string InnerToString()
    {
        return Message;
    }
}