using Irc.Messages;

namespace Messages.Replies.CommandResponses;

[Command(RPL_ENDOFWHOIS)]
public class EndOfWhoisReply : Reply
{
    public const string DefaultMessage = "End of WHOIS list";

    const string RPL_ENDOFWHOIS = "318";
    public string Nickname { get; set; }
    public string Message { get; set; }

    public EndOfWhoisReply(string sender, string target, string nickname, string message) : base(sender, target, RPL_ENDOFWHOIS)
    {
        Nickname = nickname;
        Message = message;
    }

    public override string InnerToString()
    {
        return $"{Nickname} :{Message}";
    }

    public new static EndOfWhoisReply Parse(string message)
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

        return new EndOfWhoisReply(sender, target, nickname, text);
    }
}