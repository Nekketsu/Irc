namespace Irc.Client.Wpf.Messenger.Requests;

public class TitleRequest
{
    public string Title { get; }

    public TitleRequest(string title)
    {
        Title = title;
    }
}
