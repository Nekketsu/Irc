using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    [Command(RPL_CREATIONTIME)]
    public class CreationTimeReply : Reply
    {
        const string RPL_CREATIONTIME = "329";
        public string ChannelName { get; private set; }
        public DateTime CreationTime { get; private set; }

        public CreationTimeReply(string sender, string target, string channelName, DateTime creationTime) : base(sender, target, RPL_CREATIONTIME)
        {
            ChannelName = channelName;
            CreationTime = creationTime;
        }

        public override string InnerToString()
        {
            return $"{ChannelName} :{new DateTimeOffset(CreationTime).ToUnixTimeSeconds()}";
        }

        public new static CreationTimeReply Parse(string message)
        {
            var messageSplit = message.Split();
            var sender = messageSplit[0];
            var target = messageSplit[2];
            var channelName = messageSplit[3];
            var creationTimeUnix = long.Parse(messageSplit[4].Substring(1));
            var creationTime = DateTimeOffset.FromUnixTimeSeconds(creationTimeUnix).DateTime;

            return new CreationTimeReply(sender, target, channelName, creationTime);
        }
    }
}