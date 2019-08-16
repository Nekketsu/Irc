using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    public class TopicReply : Reply
    {
        const string RPL_TOPIC = "332";
        public string ChannelName { get; private set; }
        public string Topic { get; set; }

        public TopicReply(string target, string channelName, string topic) : base(target, RPL_TOPIC)
        {
            ChannelName = channelName;
            Topic = topic;
        }

        public override string InnerToString()
        {
            return $"{ChannelName} :{Topic}";
        }
    }
}