namespace Irc.Client.Maui.Blazor.Models;

public class ChatMessage
{
    public DateTime DateTime { get; }
    public Nickname From { get; }
    public string Message { get; }

    public ChatMessage(string message)
    {
        DateTime = DateTime.Now;
        Message = message;
    }

    public ChatMessage(Nickname from, string message) : this(message)
    {
        From = from;
    }

    public override string ToString()
    {
        var dateTime = DateTime.ToString("dd:mm:ss");

        return From is null
            ? $"[{dateTime}] {Message}"
            : $"[{dateTime}] <{From}> {Message}";
    }
}
