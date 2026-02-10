namespace Irc.Messages.Messages;

[Command("NOTICE")]
public class NoticeMessage : Message
{
    public string From { get; set; }
    public string Target { get; set; }
    public string Text { get; set; }

    public NoticeMessage(string target, string text)
    {
        Target = target;
        Text = text;
    }

    public NoticeMessage(string from, string target, string text) : this(target, text)
    {
        From = from;
    }

    public override string ToString()
    {
        return $"{Command} {Target} :{Text}";
    }

    public new static NoticeMessage Parse(string message)
    {
        var messageSplit = message.Split();

        if (message.StartsWith(':'))
        {
            var from = messageSplit[0][":".Length..];
            var target = messageSplit[2];
            var text = message
[messageSplit[0].Length..].TrimStart()
[messageSplit[1].Length..].TrimStart()
[messageSplit[2].Length..].TrimStart()
[":".Length..];

            return new(from, target, text);
        }
        else
        {
            var target = messageSplit[1];
            var text = message
[messageSplit[0].Length..].TrimStart()
[messageSplit[1].Length..].TrimStart()
[":".Length..];

            return new(target, text);
        }
    }
}