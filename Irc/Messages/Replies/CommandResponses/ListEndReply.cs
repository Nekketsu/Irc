using Irc.Messages;

namespace Messages.Replies.CommandResponses;

[Command(RPL_LISTEND)]
public class ListEndReply : Reply
{
    public const string DefaultMessage = "End of LIST";

    const string RPL_LISTEND = "323";
    public string Message { get; set; }

    public ListEndReply(string sender, string target, string message) : base(sender, target, RPL_LISTEND)
    {
        Message = message;
    }

    public override string InnerToString()
    {
        return $":{Message}";
    }
}