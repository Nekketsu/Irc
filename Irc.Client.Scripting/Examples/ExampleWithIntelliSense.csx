// Example Script with IntelliSense Support
// Add these #r directives at the top for VS Code IntelliSense

#r "../../Irc/bin/Debug/net10.0/Irc.dll"
#r "../../Irc.Client/bin/Debug/net10.0/Irc.Client.dll"
#r "../../Irc.Client.Scripting/bin/Debug/net10.0/Irc.Client.Scripting.dll"

using System;
using Irc.Client.Scripting;
using Irc.Client.Scripting.Events;

// Now you have full IntelliSense!
// Type "Host." and see all available methods
// Type "e." in the handlers and see all event properties

// Handle channel messages
Host.OnChannelMessage(async (e) =>
{
    // e has full IntelliSense: Channel, From, Message
    if (e.Message.StartsWith("!hello"))
    {
        // Host has full IntelliSense: all methods visible
        await Host.SendChannelMessageAsync(e.Channel.Name, 
            $"Hello {e.From}!");
    }
});

// Handle private messages
Host.OnPrivateMessage(async (e) =>
{
    // e has full IntelliSense: From, Message
    if (e.Message.Contains("help"))
    {
        await Host.SendPrivateMessageAsync(e.From.ToString(), 
            "Available commands: !help, !info");
    }
});

// Welcome users
Host.OnUserJoined(async (e) =>
{
    // e has full IntelliSense: Channel, Nickname
    await Host.SendChannelMessageAsync(e.Channel.Name, 
        $"Welcome {e.Nickname}!");
});

// Access current user info
// Host.Me has full IntelliSense: Nickname, Username, Channels
Host.Log($"Bot name: {Host.Me.Nickname}");
Host.Log($"Connected to {Host.Me.Channels.Count} channels");

Host.Log("Script initialized with IntelliSense support");
