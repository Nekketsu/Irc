namespace Irc.Messages.Messages;

[Command("MODE")]
public class ModeMessage : Message
{
    public string From { get; set; }
    public string Target { get; }
    public string Modes { get; }

    public ModeMessage(string target)
    {
        Target = target;
    }

    public ModeMessage(string from, string target, string modes) : this(target)
    {
        From = from;
        Modes = modes;
    }

    public override string ToString()
    {
        return From is null
            ? $"{Command} {Target}"
            : $":{From} {Command} {Target} {Modes}";
    }

    public new static ModeMessage Parse(string message)
    {
        var messageSplit = message.Split();

        if (messageSplit[0].StartsWith(":"))
        {
            var from = messageSplit[0][":".Length..];
            var target = messageSplit[2];
            var modes = message
[messageSplit[0].Length..].TrimStart()
[messageSplit[1].Length..].TrimStart()
[messageSplit[2].Length..].TrimStart();

            return new(from, target, modes);
        }
        else
        {
            var target = messageSplit[1];
            return new(target);
        }
    }
}