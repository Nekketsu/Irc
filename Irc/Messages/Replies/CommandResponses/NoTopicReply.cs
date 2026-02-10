using Irc.Messages;

namespace Messages.Replies.CommandResponses;

[Command(RPL_NOTOPIC)]
public class NoTopicReply : Reply
{
    public const string DefaultMessage = "No topic is set";

    const string RPL_NOTOPIC = "331";
    public string ChannelName { get; private set; }
    public string Message { get; set; }

    public NoTopicReply(string sender, string target, string channelName, string message) : base(sender, target, RPL_NOTOPIC)
    {
        ChannelName = channelName;
        Message = message;
    }

    public override string InnerToString()
    {
        return $"{ChannelName} :{Message}";
    }
}