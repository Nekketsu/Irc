// Farewell Script
// Uses ChannelUserEventArgs to handle user parts

Host.OnUserParted(async (e) =>
{
    // e is ChannelUserEventArgs with Channel and Nickname properties
    var nickname = e.Nickname.ToString();
    var channelName = e.Channel.Name;

    await Host.SendChannelMessageAsync(channelName, 
        $"Goodbye, {nickname}! See you soon!");

    Host.Log($"{nickname} parted {channelName}");
});

Host.Log("Farewell script loaded and subscribed to user parts");
