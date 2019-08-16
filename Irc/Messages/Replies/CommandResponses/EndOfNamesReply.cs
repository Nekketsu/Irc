using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    public class EndOfNamesReply : Reply
    {
        const string RPL_ENDOFNAMES = "366";

        public string ChannelName { get; set; }
        public string Message { get; private set; }

        public EndOfNamesReply(string target, string channelName, string message) : base(target, RPL_ENDOFNAMES)
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