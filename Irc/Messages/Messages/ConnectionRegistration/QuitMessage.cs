namespace Irc.Messages.Messages
{
    public class QuitMessage : Message
    {
        public string Target { get; set; }
        public string Reason { get; set; }

        public QuitMessage(string reason)
        {
            Reason = reason;
        }

        public QuitMessage(string target, string reason) : this(reason)
        {
            Target = target;
        }

        public override string ToString()
        {
            return (Target == null)
                ? $"{Command} :{Reason}"
                : $":{Target} {Command} :{Reason}";
        }


        public new static QuitMessage Parse(string message)
        {
            var messageSplit = message.Split();

            var reason = message.Substring(message.IndexOf(messageSplit[0]) + messageSplit[0].Length).TrimStart();
            if (reason.StartsWith(':'))
            {
                reason = reason.Substring(1);
            }

            return new QuitMessage(reason);
        }
    }
}
