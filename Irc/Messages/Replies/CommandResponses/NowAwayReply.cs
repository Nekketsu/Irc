namespace Irc.Messages.Replies.CommandResponses;

[Command(RPL_NOWAWAY)]
public class NowAwayReply : Reply
{
    const string RPL_NOWAWAY = "306";

    public string Message { get; set; }

    public NowAwayReply(string sender, string target, string message) : base(sender, target, RPL_NOWAWAY)
    {
        Message = message;
    }


    public override string InnerToString()
    {
        return $": {Message}";
    }

    public new static NowAwayReply Parse(string message)
    {
        var messageSplit = message.Split();

        var sender = messageSplit[0][":".Length..];
        var target = messageSplit[2];
        var text = message
[messageSplit[0].Length..].TrimStart()
[messageSplit[1].Length..].TrimStart()
[messageSplit[2].Length..].TrimStart()
            .TrimStart(':');

        return new NowAwayReply(sender, target, text);
    }
}
