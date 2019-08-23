using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    public class ListReply : Reply
    {
        const string RPL_LIST = "322";
        public string ChannelName { get; set; }
        public int Visible { get; set; }
        public string Topic { get; set; }

        public ListReply(string target, string channelName, int visible, string topic) : base(target, RPL_LIST)
        {
            ChannelName = channelName;
            Visible = visible;
            Topic = topic;
        }

        public override string InnerToString()
        {
            return $"{ChannelName} {Visible} :{Topic ?? string.Empty}";
        }
    }
}