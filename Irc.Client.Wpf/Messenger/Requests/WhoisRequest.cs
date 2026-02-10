namespace Irc.Client.Wpf.Messenger.Requests;

public class WhoisRequest
{
    public string Nickname { get; }

    public WhoisRequest(string nickname)
    {
        Nickname = nickname;
    }
}
