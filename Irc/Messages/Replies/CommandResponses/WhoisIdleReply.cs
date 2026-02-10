using Irc.Messages;

namespace Messages.Replies.CommandResponses;

[Command(RPL_WHOISIDLE)]
public class WhoisIdleReply : Reply
{
    public const string DefaultMessage = "seconds idle";

    const string RPL_WHOISIDLE = "317";
    public string Nickname { get; set; }
    public TimeSpan Idle { get; set; }
    public DateTime SignOn { get; set; }
    public string Message { get; set; }

    public WhoisIdleReply(string sender, string target, string nickname, TimeSpan idle, string message) : base(sender, target, RPL_WHOISIDLE)
    {
        Nickname = nickname;
        Idle = idle;
        Message = message;
    }

    public override string InnerToString()
    {
        return $"{Nickname} {(long)Idle.TotalSeconds} :{Message}";
    }

    public new static WhoisIdleReply Parse(string message)
    {
        var messageSplit = message.Split();

        var sender = messageSplit[0][":".Length..];
        var target = messageSplit[2];
        var nickname = messageSplit[3];
        var idleString = messageSplit[4];
        var messageText = message.Split(':').Last();

        var idleSeconds = long.Parse(idleString);
        var idle = TimeSpan.FromSeconds(idleSeconds);

        return new WhoisIdleReply(sender, target, nickname, idle, messageText);
    }
}