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

        public new static NameReply Parse(string message)
        {
            var messageSplit = message.Split();

            var sender = messageSplit[0].TrimStart(':');
            message = message.Substring(messageSplit[0].Length).Trim();

            message = message.Substring(messageSplit[1].Length).Trim(); // Command

            var target = messageSplit[2];
            message = message.Substring(messageSplit[2].Length).Trim();

            var channelMode = messageSplit[3];
            message = message.Substring(messageSplit[3].Length).Trim();

            var channelName = messageSplit[4];
            message = message.Substring(messageSplit[4].Length).Trim().TrimStart(':');

            var nicknames = message.Split();

            return new NameReply(sender, target, channelMode, channelName, nicknames);
        }
    }
}