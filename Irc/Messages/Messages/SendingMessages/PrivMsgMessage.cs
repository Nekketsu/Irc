namespace Irc.Messages.Messages
{
    [Command("PRIVMSG")]
    public class PrivMsgMessage : Message
    {
        public string From { get; set; }
        public string Target { get; set; }
        public string Text { get; set; }

        public PrivMsgMessage(string target, string text)
        {
            Target = target;
            Text = text.StartsWith(":") ? text.Substring(1) : text;
        }

        public PrivMsgMessage(string from, string target, string text) : this(target, text)
        {
            From = from;
        }

        public override string ToString()
        {
            return (From == null)
                ? $"{Command} {Target} :{Text}"
                : $":{From} {Command} {Target} :{Text}";
        }

        public new static PrivMsgMessage Parse(string message)
        {
            var messageSplit = message.Split();

            if (message.StartsWith(':'))
            {
                var from = messageSplit[0].Substring(":".Length);
                var target = messageSplit[2];
                var text = message
                    .Substring(messageSplit[0].Length).TrimStart()
                    .Substring(messageSplit[1].Length).TrimStart()
                    .Substring(messageSplit[2].Length).TrimStart()
                    .Substring(":".Length);

                return new(from, target, text);
            }
            else
            {
                var target = messageSplit[1];
                var text = message
                    .Substring(messageSplit[0].Length).TrimStart()
                    .Substring(messageSplit[1].Length).TrimStart()
                    .Substring(":".Length);

                return new(target, text);
            }
        }
    }
}