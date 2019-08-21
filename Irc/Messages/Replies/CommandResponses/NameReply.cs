using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    public class NameReply : Reply
    {
        const string RPL_NAMREPLY = "353";
        public string ChannelName { get; set; }
        public string[] Nicknames { get; private set; }

        public NameReply(string target, string channelName, params string[] nicknames) : base(target, RPL_NAMREPLY)
        {
            ChannelName = channelName.StartsWith("#")
                ? channelName.Substring(1)
                : channelName;

            Nicknames = nicknames;
        }

        public override string InnerToString()
        {
            return $"= #{ChannelName} :{string.Join(' ', Nicknames)}";
        }
    }
}