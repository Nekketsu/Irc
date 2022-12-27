namespace Irc.Messages.Messages
{
    [Command("TOPIC")]
    public class TopicMessage : Message
    {
        public string From { get; set; }
        public string ChannelName { get; set; }
        public string Topic { get; set; }

        public TopicMessage(string channelName)
        {
            ChannelName = channelName;
        }

        public TopicMessage(string channelName, string topic) : this(channelName)
        {
            Topic = topic;
        }

        public TopicMessage(string from, string channelName, string topic) : this(channelName, topic)
        {
            From = from;
        }


        public override string ToString()
        {
            return Topic == null
                ? $"{Command} {ChannelName}"
                : From == null
                    ? $"{Command} {ChannelName} :{Topic}"
                    : $":{From} {Command} {ChannelName} :{Topic}";
        }

        public new static TopicMessage Parse(string message)
        {
            var messageSplit = message.Split();
            var channelName = messageSplit[1];

            var text = message.Substring(message.IndexOf(messageSplit[0]) + messageSplit[0].Length).TrimStart();
            text = message.Substring(message.IndexOf(messageSplit[1]) + messageSplit[1].Length).TrimStart();
            if (text.StartsWith(':'))
            {
                var topic = text.Substring(1);
                return new TopicMessage(channelName, topic);
            }

            return new TopicMessage(channelName);
        }
    }
}