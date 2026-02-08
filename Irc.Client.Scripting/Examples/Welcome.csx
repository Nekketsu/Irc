// Welcome Script
// Uses ChannelUserEventArgs to handle user joins

Host.OnUserJoined(async (e) =>
{
    // e is ChannelUserEventArgs with Channel and Nickname properties
    var nickname = e.Nickname.ToString();
    var channelName = e.Channel.Name;

    await Host.SendChannelMessageAsync(channelName, 
        $"Welcome to {channelName}, {nickname}! Enjoy your stay!");

    Host.Log($"Welcomed {nickname} to {channelName}");
});

Host.Log("Welcome script loaded and subscribed to user joins");
