namespace Irc.Messages.Messages;

[Command("USERHOST")]
public class UserhostMessage : Message
{
    public string Nickname { get; set; }

    public UserhostMessage(string nickname)
    {
        Nickname = nickname;
    }

    public override string ToString()
    {
        return $"{Command} {Nickname}";
    }
}