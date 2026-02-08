namespace Irc.Client.Scripting.Api
{
    /// <summary>
    /// Read-only wrapper for local user information.
    /// </summary>
    internal class LocalUserInfo : ILocalUserInfo
    {
        private readonly LocalUser localUser;

        public LocalUserInfo(LocalUser localUser)
        {
            this.localUser = localUser;
        }

        public string Nickname => localUser.Nickname.ToString();

        public IReadOnlyList<IChannelInfo> Channels =>
            localUser.Channels.Select(c => new ChannelInfo(c)).ToList().AsReadOnly();

        public string RealName => string.Empty; // Not available in current User model

        public string Username => localUser.Username ?? string.Empty;
    }

    /// <summary>
    /// Read-only wrapper for channel information.
    /// </summary>
    public class ChannelInfo : IChannelInfo
    {
        private readonly Channel channel;

        public ChannelInfo(Channel channel)
        {
            this.channel = channel;
        }

        public string Name => channel.Name;

        public IReadOnlyList<IUserInfo> Users =>
            channel.Users.Select(u => new UserInfo(u)).ToList().AsReadOnly();

        public string Topic => channel.Topic ?? string.Empty;

        public int UserCount => channel.Users.Count();

        internal static ChannelInfo FromChannel(Channel channel) => new ChannelInfo(channel);
    }

    /// <summary>
    /// Read-only wrapper for user information.
    /// </summary>
    public class UserInfo : IUserInfo
    {
        private readonly User? user;
        private readonly string nickname;

        public UserInfo(User user)
        {
            this.user = user;
            this.nickname = user.Nickname.ToString();
        }

        private UserInfo(string nickname)
        {
            this.user = null;
            this.nickname = nickname;
        }

        public string Nickname => nickname;

        public string Username => user?.Username ?? string.Empty;

        public string Hostname => user?.Host ?? string.Empty;

        internal static UserInfo FromUser(User user) => new UserInfo(user);

        internal static UserInfo FromNickname(Nickname nickname) => new UserInfo(nickname.ToString());
    }
}
