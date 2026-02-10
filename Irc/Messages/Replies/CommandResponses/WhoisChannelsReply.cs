using Irc.Messages;

namespace Messages.Replies.CommandResponses;

[Command(RPL_WHOISCHANNELS)]
public class WhoisChannelsReply : Reply
{
    const string RPL_WHOISCHANNELS = "319";
    public string Nickname { get; set; }
    public IEnumerable<string> ChannelNames { get; set; }

    public WhoisChannelsReply(string sender, string target, string nickname, IEnumerable<string> channelNames) : base(sender, target, RPL_WHOISCHANNELS)
    {
        Nickname = nickname;
        ChannelNames = channelNames;
    }

    public override string InnerToString()
    {
        return $"{Nickname} :{string.Join(" ", ChannelNames)}";
    }

    public new static WhoisChannelsReply Parse(string message)
    {
        var messageSplit = message.Split();

        var sender = messageSplit[0][":".Length..]; ;
        var target = messageSplit[2];
        var nickname = messageSplit[3];
        var channelsText = message
[messageSplit[0].Length..].TrimStart()
[messageSplit[1].Length..].TrimStart()
[messageSplit[2].Length..].TrimStart()
[messageSplit[3].Length..].TrimStart()
            .TrimStart(':');

        var channels = channelsText.Split();

        return new WhoisChannelsReply(sender, target, nickname, channels);
    }
}