// Channel Logger Script
// Uses ChannelMessageEventArgs to log all channel messages

Host.OnChannelMessage(async (e) =>
{
    // e is ChannelMessageEventArgs with Channel, From, and Message properties
    var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    var channelName = e.Channel.Name;
    var senderName = e.From.ToString();
    var message = e.Message;

    Host.Log($"[{timestamp}] {channelName} <{senderName}> {message}");
    
    await Task.CompletedTask;
});

Host.Log("ChannelLogger script loaded");
