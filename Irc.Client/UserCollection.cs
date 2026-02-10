using System.Collections;

namespace Irc.Client;

public class UserCollection : IEnumerable<User>
{
    public Dictionary<Nickname, User> users;

    public UserCollection()
    {
        users = [];
    }

    public User this[Nickname nickname]
    {
        get => users.GetValueOrDefault(nickname);
        set => users[nickname] = value;
    }

    public IEnumerator<User> GetEnumerator()
    {
        return users.Values
            .OrderBy(user => (string)user.Nickname, NicknameComparer.Default)
            .GetEnumerator();
    }

    internal bool Remove(Nickname nickname)
    {
        return users.Remove(nickname);
    }

    public bool Contains(Nickname nickname)
    {
        return users.ContainsKey(nickname);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
