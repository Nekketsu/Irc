using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    [Command(RPL_ENDOFNAMES)]
    public class EndOfNamesReply : Reply
    {
        public const string DefaultMessage = "End of NAMES list";

        const string RPL_ENDOFNAMES = "366";
        public string ChannelName { get; set; }
        public string Message { get; private set; }

        public EndOfNamesReply(string sender, string target, string channelName, string message) : base(sender, target, RPL_ENDOFNAMES)
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