namespace Irc.Messages.Messages
{
    [Command("JOIN")]
    public class JoinMessage : Message
    {
        public string ChannelName { get; set; }
        public string From { get; set; }

        public JoinMessage(string channelName)
        {
            ChannelName = channelName.TrimStart(':');
        }

        public JoinMessage(string from, string channelName) : this(channelName)
        {
            From = from;
        }

        public override string ToString()
        {
            return (From == null)
                ? $"{Command} {ChannelName}"
                : $":{From} {Command} {ChannelName}";
        }
    }
}