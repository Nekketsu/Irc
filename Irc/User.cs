public class User
{
    public string UserName { get; private set; }
    public string Mode { get; private set; }
    public string Unused { get; private set; }
    public string RealName { get; private set; }

    public User(string user, string mode, string unused, string realName)
    {
        UserName = user;
        Mode = mode;
        Unused = unused;
        RealName = realName;
    }
}