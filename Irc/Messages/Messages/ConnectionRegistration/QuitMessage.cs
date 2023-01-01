namespace Irc.Messages.Messages
{
    [Command("QUIT")]
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
            string target = null;
            int index = 0;

            var messageSplit = message.Split();

            if (messageSplit[0].StartsWith(':'))
            {
                target = messageSplit[index].Substring(1);
                message = message.Substring(messageSplit[index].Length).TrimStart();
                index++;
            }

            var reason = message.Substring(messageSplit[index].Length).TrimStart().TrimStart(':');

            return target is null
                ? new QuitMessage(reason)
                : new QuitMessage(target, reason);
        }
    }
}
