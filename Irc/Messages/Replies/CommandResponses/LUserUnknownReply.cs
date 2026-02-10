using Irc.Messages;

namespace Messages.Replies.CommandResponses;

[Command(RPL_LUSERUNKNOWN)]
public class LUserUnknownReply : Reply
{
    const string RPL_LUSERUNKNOWN = "253";

    public int UnknownConnectionsCount { get; }

    public LUserUnknownReply(string sender, string target, int unknownConnectionsCount) : base(sender, target, RPL_LUSERUNKNOWN)
    {
        UnknownConnectionsCount = unknownConnectionsCount;
    }

    public override string InnerToString()
    {
        return $"{UnknownConnectionsCount} :unknown connection(s)";
    }

    public new static LUserUnknownReply Parse(string message)
    {
        var messageSplit = message.Split();

        var sender = messageSplit[0][":".Length..];
        var target = messageSplit[2];
        var unknownConnectionsCount = int.Parse(messageSplit[3]);

        return new(sender, target, unknownConnectionsCount);
    }
}