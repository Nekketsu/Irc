using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    [Command(RPL_NAMREPLY)]
    public class NameReply : Reply
    {
        const string RPL_NAMREPLY = "353";
        public string ChannelMode { get; set; }
        public string ChannelName { get; set; }
        public string[] Nicknames { get; private set; }

        public NameReply(string sender, string target, string channelMode, string channelName, params string[] nicknames) : base(sender, target, RPL_NAMREPLY)
        {
            ChannelMode = channelMode;
            ChannelName = channelName;

            Nicknames = nicknames;
        }

        public override string InnerToString()
        {
            return $"{ChannelMode} {ChannelName} :{string.Join(' ', Nicknames)}";
        }
    }
}