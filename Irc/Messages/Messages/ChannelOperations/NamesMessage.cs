namespace Irc.Messages.Messages
{
    [Command("NAMES")]
    public class NamesMessage : Message
    {
        public string ChannelName { get; set; }

        public NamesMessage(string channelName)
        {
            ChannelName = channelName;
        }

        public override string ToString()
        {
            return $"{Command} {ChannelName}";
        }
    }
}