using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    [Command(RPL_CHANNELMODEIS)]
    public class ChannelModeIsReply : Reply
    {
        const string RPL_CHANNELMODEIS = "324";
        public string ChannelName { get; private set; }
        public string Mode { get; set; }
        public string ModeParams { get; set; }

        public ChannelModeIsReply(string sender, string target, string channelName, string mode, string modeParams = null) : base(sender, target, RPL_CHANNELMODEIS)
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