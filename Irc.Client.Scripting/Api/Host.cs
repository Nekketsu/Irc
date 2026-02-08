using Irc.Client.Scripting.Api.Events;

namespace Irc.Client.Scripting.Api;

/// <summary>
/// Static global access to IRC scripting functionality.
/// Available in all scripts without needing declaration.
/// </summary>
public static class Host
{
    [ThreadStatic]
    private static ScriptHost? _current;

    /// <summary>
    /// Gets or sets the current script host instance for this thread.
    /// </summary>
    internal static ScriptHost? Current
    {
        get => _current;
        set => _current = value;
    }

    private static ScriptHost GetHost()
    {
        if (_current is null)
            throw new InvalidOperationException("No ScriptHost is available in the current context. This should not happen during script execution.");
        return _current;
    }

    /// <summary>
    /// Gets information about the local user.
    /// </summary>
    public static ILocalUserInfo Me => GetHost().Me;

    /// <summary>
    /// Subscribes to channel message events.
    /// </summary>
    /// <param name="handler">Handler function that receives channel message events</param>
    public static void OnChannelMessage(Func<ChannelMessageEventArgs, Task> handler)
        => GetHost().OnChannelMessage(handler);

    /// <summary>
    /// Subscribes to private message events.
    /// </summary>
    /// <param name="handler">Handler function that receives private message events</param>
    public static void OnPrivateMessage(Func<PrivateMessageEventArgs, Task> handler)
        => GetHost().OnPrivateMessage(handler);

    /// <summary>
    /// Subscribes to user joined channel events.
    /// </summary>
    /// <param name="handler">Handler function that receives user joined events</param>
    public static void OnUserJoined(Func<ChannelUserEventArgs, Task> handler)
        => GetHost().OnUserJoined(handler);

    /// <summary>
    /// Subscribes to user parted channel events.
    /// </summary>
    /// <param name="handler">Handler function that receives user parted events</param>
    public static void OnUserParted(Func<ChannelUserEventArgs, Task> handler)
        => GetHost().OnUserParted(handler);

    /// <summary>
    /// Sends a message to a channel.
    /// </summary>
    /// <param name="channelName">Name of the channel</param>
    /// <param name="message">Message to send</param>
    public static Task SendChannelMessageAsync(string channelName, string message)
        => GetHost().SendChannelMessageAsync(channelName, message);

    /// <summary>
    /// Sends a private message to a user.
    /// </summary>
    /// <param name="nickname">Nickname of the user</param>
    /// <param name="message">Message to send</param>
    public static Task SendPrivateMessageAsync(string nickname, string message)
        => GetHost().SendPrivateMessageAsync(nickname, message);

    /// <summary>
    /// Joins a channel.
    /// </summary>
    /// <param name="channelName">Name of the channel to join</param>
    public static Task JoinChannelAsync(string channelName)
        => GetHost().JoinChannelAsync(channelName);

    /// <summary>
    /// Leaves a channel.
    /// </summary>
    /// <param name="channelName">Name of the channel to leave</param>
    /// <param name="reason">Optional reason for leaving</param>
    public static Task PartChannelAsync(string channelName, string? reason = null)
        => GetHost().PartChannelAsync(channelName, reason);

    /// <summary>
    /// Logs a message to the script output.
    /// </summary>
    /// <param name="message">Message to log</param>
    public static void Log(string message)
        => GetHost().Log(message);
}
