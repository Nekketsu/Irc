// Auto Responder Script
// Uses MessageEventArgs to handle private messages

Host.OnPrivateMessage(async (e) =>
{
    // e is MessageEventArgs with From and Message properties
    var senderName = e.From.ToString();
    var message = e.Message;
    
    if (message.Trim().Equals("!ping", StringComparison.OrdinalIgnoreCase))
    {
        await Host.SendPrivateMessageAsync(senderName, "Pong!");
    }
    else if (message.Trim().Equals("!time", StringComparison.OrdinalIgnoreCase))
    {
        var currentTime = DateTime.Now.ToString("HH:mm:ss");
        await Host.SendPrivateMessageAsync(senderName, $"Current time: {currentTime}");
    }
    else if (message.Trim().Equals("!info", StringComparison.OrdinalIgnoreCase))
    {
        await Host.SendPrivateMessageAsync(senderName, 
            "I'm a scripted IRC bot. Try: !ping, !time, !info");
    }
});

Host.Log("AutoResponder script loaded and subscribed to private messages");
