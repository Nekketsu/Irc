namespace Irc.Client.Events;

public class NickChangedEventArgs : EventArgs
{
    public Nickname OldNickname { get; }
    public Nickname NewNickname { get; }

    public NickChangedEventArgs(Nickname oldNickname, Nickname newNickname)
    {
        OldNickname = oldNickname;
        NewNickname = newNickname;
    }
}
