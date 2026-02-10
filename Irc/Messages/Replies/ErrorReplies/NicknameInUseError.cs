namespace Irc.Messages.Replies.ErrorReplies;

[Command("433")]
public class NicknameInUseError : Reply
{
    public const string ERR_NICKNAMEINUSE = "433";

    const string DefaultMessage = "Nickname is already in use";

    public string Nickname { get; }
    public string Message { get; }

    public NicknameInUseError(string sender, string target, string nickname) : base(sender, target, ERR_NICKNAMEINUSE)
    {
        Nickname = nickname;
        Message = DefaultMessage;
    }

    public NicknameInUseError(string sender, string target, string nickname, string message) : base(sender, target, ERR_NICKNAMEINUSE)
    {
        Nickname = nickname;
        Message = message;
    }

    public override string InnerToString()
    {
        return $":{Message}";
    }

    public new static NicknameInUseError Parse(string message)
    {
        var messageSplit = message.Split();

        var sender = messageSplit[0];
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
