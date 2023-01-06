namespace Irc.Messages.Messages
{
    [Command("KICK")]
    public class KickMessage : Message
    {
        public string From { get; set; }
        public string ChannelName { get; set; }
        public string Nickname { get; set; }
        public string Message { get; set; }

        public KickMessage(string channelName, string nickname, string message)
        {
            ChannelName = channelName;
            Nickname = nickname;
            Message = message;
        }
        public KickMessage(string from, string channelName, string nickname, string message) : this(channelName, nickname, message)
        {
            From = from;
        }

        public override string ToString()
        {
            return From == null
                ? $"{Command} {ChannelName} :{Nickname}"
                : $":{From} {Command} {ChannelName} :{Nickname}";
        }

        public new static KickMessage Parse(string message)
        {
            var messageSplit = message.Split();

            var from = messageSplit[0];
            var channelName = messageSplit[2];
            var nickname = messageSplit[3];
            var text = message
                .Substring(messageSplit[0].Length).TrimStart()
                .Substring(messageSplit[1].Length).TrimStart()
                .Substring(messageSplit[2].Length).TrimStart()
                .Substring(messageSplit[3].Length).TrimStart()
                .Substring(":".Length);

            return new KickMessage(from, channelName, nickname, text);
        }
    }
}