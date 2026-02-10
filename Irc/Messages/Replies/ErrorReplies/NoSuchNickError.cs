using Irc.Messages;

namespace Messages.Replies.ErrorReplies;

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

    public new static NoSuchNickError Parse(string message)
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
[":".Length..];

        return new(sender, target, nickname, text);
    }
}