namespace Irc.Client.Events;

public class UserEventArgs : EventArgs
{
    public Nickname Nickname { get; }

    public UserEventArgs(Nickname nickname)
    {
        Nickname = nickname;
    }
}
