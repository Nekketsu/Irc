namespace Irc.Messages.Messages
{
    [Command("MODE")]
    public class ModeMessage : Message
    {
        public string ChannelName { get; set; }

        public ModeMessage(string channelName)
        {
            ChannelName = channelName;
        }

        public override string ToString()
        {
            return $"{Command} #{ChannelName}";
        }
    }
}