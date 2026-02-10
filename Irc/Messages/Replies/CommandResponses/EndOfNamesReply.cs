using Irc.Messages;

namespace Messages.Replies.CommandResponses;

[Command(RPL_ENDOFNAMES)]
public class EndOfNamesReply : Reply
{
    public const string DefaultMessage = "End of NAMES list";

    const string RPL_ENDOFNAMES = "366";
    public string ChannelName { get; set; }
    public string Message { get; private set; }

    public EndOfNamesReply(string sender, string target, string channelName, string message) : base(sender, target, RPL_ENDOFNAMES)
    {
        ChannelName = channelName;
        Message = message;
    }

    public override string InnerToString()
    {
        return $"{ChannelName} :{Message}";
    }

    public new static EndOfNamesReply Parse(string message)
    {
        var messageSplit = message.Split();

        var sender = messageSplit[0][":".Length..];
        var target = messageSplit[2];
        var channelName = messageSplit[3];
        var text = message
[messageSplit[0].Length..][":".Length..]
[messageSplit[1].Length..][":".Length..]
[messageSplit[2].Length..][":".Length..]
[messageSplit[3].Length..][":".Length..]
[":".Length..];

        return new(sender, target, channelName, text);
    }
}