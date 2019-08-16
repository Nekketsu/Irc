using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    public class NameReply : Reply
    {
        const string RPL_NAMREPLY = "353";
        public string ChannelName { get; set; }
        public string[] NickNames { get; private set; }

        public NameReply(string target, string channelName, params string[] nickNames) : base(target, RPL_NAMREPLY)
        {
            ChannelName = channelName.StartsWith("#")
                ? channelName.Substring(1)
                : channelName;

            NickNames = nickNames;
        }

        public override string InnerToString()
        {
            return $"= #{ChannelName} :{string.Join(' ', NickNames)}";
        }
    }
}