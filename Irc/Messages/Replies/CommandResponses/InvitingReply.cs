using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    public class InvitingReply : Reply
    {
        const string RPL_INVITING = "341";
        public string ChannelName { get; set; }
        public string Nickname { get; set; }

        public InvitingReply(string target, string channelName, string nickname) : base(target, RPL_INVITING)
        {
            ChannelName = channelName;
            Nickname = nickname;
        }

        public override string InnerToString()
        {
            return $"{Nickname} {ChannelName}";
        }
    }
}