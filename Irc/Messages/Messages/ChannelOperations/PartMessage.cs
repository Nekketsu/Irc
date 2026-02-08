namespace Irc.Messages.Messages
{
    [Command("PART")]
    public class PartMessage : Message
    {
        public string From { get; set; }
        public string ChannelName { get; set; }
        public string Message { get; set; }

        public PartMessage(string channelName)
        {
            ChannelName = channelName;
        }

        public PartMessage(string channelName, string message) : this(channelName)
        {
            Message = message?.StartsWith(':') ?? false
                ? message.Substring(1)
                : message;
        }

        public PartMessage(string from, string channelName, string message) : this(channelName, message)
        {
            From = from;
        }

        public override string ToString()
        {
            var text = (From is null)
                ? $"{Command} {ChannelName}"
                : $":{From} {Command} {ChannelName}";

            if (Message is not null)
            {
                text = $"{text} :{Message}";
            }

            return text;
        }

        public new static PartMessage Parse(string message)
        {
            string from = null;
            int index = 0;

            var messageSplit = message.Split();

            if (messageSplit[0].StartsWith(':'))
            {
                from = messageSplit[index].Substring(1);
                message = message.Substring(messageSplit[index].Length).TrimStart();
                index++;
            }

            message = message.Substring(messageSplit[index].Length).TrimStart(); // Command

            var channelName = messageSplit[index + 1].TrimStart(':');

            message = message.Substring(messageSplit[index + 1].Length).TrimStart().TrimStart(':');

            return from is null
                ? new PartMessage(channelName, message)
                : new PartMessage(from, channelName, message);
        }
    }
}