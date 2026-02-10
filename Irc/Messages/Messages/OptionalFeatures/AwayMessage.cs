namespace Irc.Messages.Messages.OptionalFeatures;

[Command("AWAY")]
public class AwayMessage : Message
{
    public string Text { get; set; }

    public AwayMessage() { }

    public AwayMessage(string text)
    {
        Text = text;
    }

    public override string ToString()
    {
        return string.IsNullOrEmpty(Text)
            ? $"{Command}"
            : $"{Command} :{Text}";
    }

    public new static AwayMessage Parse(string message)
    {
        var messageSplit = message.Split();

        if (messageSplit.Length == 1)
        {
            return new AwayMessage();
        }

        var text = message
[messageSplit[0].Length..].TrimStart()
[":".Length..];

        return new AwayMessage(message);
    }
}
