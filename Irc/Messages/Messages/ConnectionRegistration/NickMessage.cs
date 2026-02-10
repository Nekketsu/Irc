namespace Irc.Messages.Messages;

[Command("NICK")]
public class NickMessage : Message
{
    public string Nickname { get; private set; }
    public string From { get; }

    public NickMessage(string nickname)
    {
        Nickname = nickname;
    }

    public NickMessage(string From, string nickname) : this(nickname)
    {
        this.From = From;
    }

    public override string ToString()
    {
        return $"{Command} {Nickname}";
    }

    public new static NickMessage Parse(string message)
    {
        var messageSplit = message.Split();

        if (messageSplit.Length == 2)
        {
            var nickname = messageSplit[1];
            return new NickMessage(nickname);
        }
        else
        {
            var from = messageSplit[0][":".Length..];
            var nickname = message
[messageSplit[0].Length..].TrimStart()
[messageSplit[1].Length..].TrimStart()
[":".Length..];

            return new NickMessage(from, nickname);
        }
    }
}