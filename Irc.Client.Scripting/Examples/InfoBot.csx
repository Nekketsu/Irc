// Info Script
// Demonstrates using Host.Me to access read-only user information

Host.OnChannelMessage(async (e) =>
{
    var msg = e.Message.Trim();
    var channelName = e.Channel.Name;

    if (msg.Equals("!whoami", StringComparison.OrdinalIgnoreCase))
    {
        // Access local user information
        var myNickname = Host.Me.Nickname;
        var myUsername = Host.Me.Username;
        var channelCount = Host.Me.Channels.Count;

        await Host.SendChannelMessageAsync(channelName,
            $"I am {myNickname} (user: {myUsername}), currently in {channelCount} channel(s).");
    }
    else if (msg.Equals("!channels", StringComparison.OrdinalIgnoreCase))
    {
        // List all channels the bot is in
        var channels = string.Join(", ", Host.Me.Channels.Select(c => c.Name));
        await Host.SendChannelMessageAsync(channelName,
            $"I'm currently in: {channels}");
    }
    else if (msg.Equals("!stats", StringComparison.OrdinalIgnoreCase))
    {
        // Show statistics about current channel
        var channel = Host.Me.Channels.FirstOrDefault(c => c.Name == channelName);
        if (channel is not null)
        {
            await Host.SendChannelMessageAsync(channelName,
                $"Channel {channel.Name}: {channel.UserCount} users, Topic: {channel.Topic}");
        }
    }
});

Host.Log("Info script loaded - try !whoami, !channels, !stats");
