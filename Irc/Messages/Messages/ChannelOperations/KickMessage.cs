namespace Irc.Messages.Messages
{
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
            var channelName = messageSplit[1];
            var nickname = messageSplit[2];

            var text = message.Substring(message.IndexOf(messageSplit[0]) + messageSplit[0].Length).TrimStart();
            text = message.Substring(message.IndexOf(messageSplit[1]) + messageSplit[1].Length).TrimStart();
            text = message.Substring(message.IndexOf(messageSplit[2]) + messageSplit[2].Length).TrimStart();

            if (text.Length == 0)
            {
                text = null;
            }

            return new KickMessage(channelName, nickname, text);
        }
    }
}