namespace Irc.Client.Scripting.Api
{
    /// <summary>
    /// Read-only interface for accessing local user information from scripts.
    /// </summary>
    public interface ILocalUserInfo
    {
        /// <summary>
        /// Gets the current nickname of the local user.
        /// </summary>
        string Nickname { get; }

        /// <summary>
        /// Gets the channels the local user is currently in.
        /// </summary>
        IReadOnlyList<IChannelInfo> Channels { get; }

        /// <summary>
        /// Gets the real name of the local user.
        /// </summary>
        string RealName { get; }

        /// <summary>
        /// Gets the username of the local user.
        /// </summary>
        string Username { get; }
    }

    /// <summary>
    /// Read-only interface for accessing channel information from scripts.
    /// </summary>
    public interface IChannelInfo
    {
        /// <summary>
        /// Gets the name of the channel.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the users in this channel.
        /// </summary>
        IReadOnlyList<IUserInfo> Users { get; }

        /// <summary>
        /// Gets the topic of the channel.
        /// </summary>
        string Topic { get; }

        /// <summary>
        /// Gets the number of users in the channel.
        /// </summary>
        int UserCount { get; }
    }

    /// <summary>
    /// Read-only interface for accessing user information from scripts.
    /// </summary>
    public interface IUserInfo
    {
        /// <summary>
        /// Gets the nickname of the user.
        /// </summary>
        string Nickname { get; }

        /// <summary>
        /// Gets the username of the user.
        /// </summary>
        string Username { get; }

        /// <summary>
        /// Gets the hostname of the user.
        /// </summary>
        string Hostname { get; }
    }
}
