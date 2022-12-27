namespace Irc.Messages.Messages
{
    [Command("MODE")]
    public class ModeMessage : Message
    {
        public static string Command = "MODE";

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