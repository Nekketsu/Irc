using Irc.Client.Events;

namespace Irc.Client;

public class User
{
    public Nickname Nickname { get; set; }
    public string Username { get; set; }
    public string Host { get; set; }

    public ChannelCollection Channels { get; }

    public event EventHandler<CommentEventArgs> Quit;
    public event EventHandler<NickChangedEventArgs> NickChanged;

    public User()
    {
        Channels = new();
    }

    public static implicit operator User(string nickname)
    {
        if (nickname is null)
        {
            return null;
        }

        var nicknameSplit = nickname.Split('!', '@');

        var nick = nicknameSplit[0];
        var user = nicknameSplit.Length == 3
            ? nicknameSplit[2]
            : null;
        var host = nicknameSplit.Length > 1
            ? nicknameSplit.Last()
            : null;

        return new User { Nickname = nick, Username = user, Host = host };
    }

    internal void OnQuit(string comment)
    {
        Quit?.Invoke(this, new(comment));
    }

    internal void OnNicknameChanged(Nickname oldNickname, Nickname newNickname)
    {
        NickChanged?.Invoke(this, new(oldNickname, newNickname));
    }
}
