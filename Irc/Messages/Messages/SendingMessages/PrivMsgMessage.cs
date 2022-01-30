using System.Linq;

namespace Irc.Messages.Messages
{
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
            var text = message;
            var messageSplit = text.Split();

            string from = null;
            if (messageSplit[0].StartsWith(":"))
            {
                from = messageSplit[0].Substring(1);
                text = text.Substring(messageSplit[0].Length).TrimStart();
                messageSplit = messageSplit.Skip(1).ToArray();
            }

            var target = messageSplit[1];

            text = text.Substring(messageSplit[0].Length).TrimStart();
            text = text.Substring(messageSplit[1].Length).TrimStart();

            var privMsgMessage = from == null
                ? new PrivMsgMessage(target, text)
                : new PrivMsgMessage(from, target, text);

            return privMsgMessage;
        }
    }
}