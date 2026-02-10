namespace Irc.Messages.Messages;

[Command("PONG")]
public class PongMessage : Message
{
    public string Server { get; set; }

    public PongMessage(string server)
    {
        var delimiter = ":";

        Server = (server.StartsWith(delimiter))
            ? server[delimiter.Length..]
            : server;
    }

    public override string ToString()
    {
        return $"PONG :{Server}";
    }
}