using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
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
    }
}