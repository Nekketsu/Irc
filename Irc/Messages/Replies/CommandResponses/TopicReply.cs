using Irc.Messages;

namespace Messages.Replies.CommandResponses;

[Command(RPL_TOPIC)]
public class TopicReply : Reply
{
    const string RPL_TOPIC = "332";
    public string ChannelName { get; private set; }
    public string Topic { get; set; }

    public TopicReply(string sender, string target, string channelName, string topic) : base(sender, target, RPL_TOPIC)
    {
        ChannelName = channelName;
        Topic = topic;
    }

    public override string InnerToString()
    {
        return $"{ChannelName} :{Topic}";
    }

    public new static TopicReply Parse(string message)
    {
        var messageSplit = message.Split();

        var sender = messageSplit[0][":".Length..];
        var target = messageSplit[2];
        var channelName = messageSplit[3];
        var topic = message
[messageSplit[0].Length..].TrimStart()
[messageSplit[1].Length..].TrimStart()
[messageSplit[2].Length..].TrimStart()
[messageSplit[3].Length..].TrimStart()
            .TrimStart(':');

        return new TopicReply(sender, target, channelName, topic);
    }
}