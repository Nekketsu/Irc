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
    }
}