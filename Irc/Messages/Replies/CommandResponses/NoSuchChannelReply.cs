using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    public class NoSuchChannelReply : Reply
    {
        const string ERR_NOSUCHCHANNEL = "403";
        public string ChannelName { get; set; }
        public string Message { get; private set; }

        public NoSuchChannelReply(string target, string channelName, string message) : base(target, ERR_NOSUCHCHANNEL)
        {
            ChannelName = channelName;
            Message = message;
        }

        public override string InnerToString()
        {
            return $"{ChannelName} :{Message}";
        }
    }
}