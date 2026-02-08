// PingPong Script
// Uses ChannelMessageEventArgs to handle channel commands

Host.OnChannelMessage(async (e) =>
{
    // e is ChannelMessageEventArgs with Channel, From, and Message properties
    var msg = e.Message.Trim();
    var channelName = e.Channel.Name;
    var senderName = e.From.ToString();

    if (msg.Equals("!ping", StringComparison.OrdinalIgnoreCase))
    {
        await Host.SendChannelMessageAsync(channelName, "Pong!");
    }
    else if (msg.StartsWith("!echo ", StringComparison.OrdinalIgnoreCase))
    {
        var echoText = msg.Substring(6);
        await Host.SendChannelMessageAsync(channelName, echoText);
    }
    else if (msg.Equals("!users", StringComparison.OrdinalIgnoreCase))
    {
        var userCount = e.Channel.Users.Count;
        await Host.SendChannelMessageAsync(channelName, 
            $"There are {userCount} users in this channel.");
    }
});

Host.Log("PingPong script loaded with channel commands");
