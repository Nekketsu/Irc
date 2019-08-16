using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    public class ChannelModeIsReply : Reply
    {
        const string RPL_CHANNELMODEIS = "324";
        public string ChannelName { get; private set; }
        public string Mode { get; set; }
        public string ModeParams { get; set; }

        public ChannelModeIsReply(string target, string channelName, string mode, string modeParams = null) : base(target, RPL_CHANNELMODEIS)
        {
            ChannelName = channelName;
            Mode = mode;
            ModeParams = modeParams;
        }

        public override string InnerToString()
        {
            var text = $"{ChannelName} :{Mode}";
            if (ModeParams != null)
            {
                text = $"{text} {ModeParams}";
            }

            return text;
        }
    }
}