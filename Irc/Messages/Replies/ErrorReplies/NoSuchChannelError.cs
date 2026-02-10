using Irc.Messages;

namespace Messages.Replies.ErrorReplies;

[Command(ERR_NOSUCHCHANNEL)]
public class NoSuchChannelError : Reply
{
    public const string DefaultMessage = "No such channel";

    const string ERR_NOSUCHCHANNEL = "403";
    public string ChannelName { get; set; }
    public string Message { get; private set; }

    public NoSuchChannelError(string sender, string target, string channelName, string message) : base(sender, target, ERR_NOSUCHCHANNEL)
    {
        ChannelName = channelName;
        Message = message;
    }

    public override string InnerToString()
    {
        return $"{ChannelName} :{Message}";
    }
}