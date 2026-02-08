namespace Irc.Messages.Messages
{
    [Command("INVITE")]
    public class InviteMessage : Message
    {
        public string From { get; set; }
        public string Nickname { get; set; }
        public string ChannelName { get; set; }

        public InviteMessage(string nickname, string channelName)
        {
            Nickname = nickname;
            ChannelName = channelName;
        }

        public InviteMessage(string from, string nickname, string channelName) : this(nickname, channelName)
        {
            From = from;
        }

        public override string ToString()
        {
            return From is null
                ? $"{Command} {Nickname} {ChannelName}"
                : $":{From} {Command} {Nickname} {ChannelName}";
        }
    }
}