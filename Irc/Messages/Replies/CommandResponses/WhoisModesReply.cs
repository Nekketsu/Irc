namespace Irc.Messages.Replies.CommandResponses;

[Command(RPL_WHOISMODES)]
public class WhoisModesReply : Reply
{
    const string RPL_WHOISMODES = "379";

    public string Nickname { get; set; }
    public string Text { get; set; }

    public WhoisModesReply(string sender, string target, string nickname, string text) : base(sender, target, RPL_WHOISMODES)
    {
        Nickname = nickname;
        Text = text;
    }

    public override string InnerToString()
    {
        return $"{Nickname} :{Text}";
    }

    public new static WhoisModesReply Parse(string message)
    {

        var messageSplit = message.Split();

        var sender = messageSplit[0][":".Length..];
        var target = messageSplit[2];
        var nickname = messageSplit[3];
        var text = message
[messageSplit[0].Length..].TrimStart()
[messageSplit[1].Length..].TrimStart()
[messageSplit[2].Length..].TrimStart()
[messageSplit[3].Length..].TrimStart()
            .TrimStart(':');

        return new WhoisModesReply(sender, target, nickname, text);
    }
}
