using Irc.Messages;

namespace Messages.Replies.CommandResponses;

[Command(RPL_INVITING)]
public class InvitingReply : Reply
{
    const string RPL_INVITING = "341";
    public string ChannelName { get; set; }
    public string Nickname { get; set; }

    public InvitingReply(string sender, string target, string channelName, string nickname) : base(sender, target, RPL_INVITING)
    {
        ChannelName = channelName;
        Nickname = nickname;
    }

    public override string InnerToString()
    {
        return $"{Nickname} {ChannelName}";
    }
}