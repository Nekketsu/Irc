namespace Irc.Messages.Messages;

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
        return (Target is null)
            ? $"{Command} :{Reason}"
            : $":{Target} {Command} :{Reason}";
    }


    public new static QuitMessage Parse(string message)
    {
        string target = null;
        var index = 0;

        var messageSplit = message.Split();

        if (messageSplit[0].StartsWith(':'))
        {
            target = messageSplit[index][1..];
            message = message[messageSplit[index].Length..].TrimStart();
            index++;
        }

        var reason = message[messageSplit[index].Length..].TrimStart().TrimStart(':');

        return target is null
            ? new QuitMessage(reason)
            : new QuitMessage(target, reason);
    }
}
