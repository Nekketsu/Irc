// ========================================
// IRC SCRIPT TEMPLATES
// ========================================
// Choose the template that matches your event type
// and customize it for your needs.
//
// Each script responds to ONE specific event type.
// Available event types:
//   - PrivateMessageReceived
//   - ChannelMessageReceived
//   - UserJoinedChannel
//   - UserPartedChannel
// ========================================


// ========================================
// TEMPLATE 1: Private Message Handler
// ========================================
// Event: PrivateMessageReceived
// Available variables:
//   - Sender: User      (the user who sent the message)
//   - Message: string   (the message content)
//   - Host: ScriptHost  (API for actions)

if (Message.Contains("hello", StringComparison.OrdinalIgnoreCase))
{
    await Host.SendPrivateMessageAsync(
        Sender.Nickname.ToString(), 
        "Hello! How can I help you?"
    );
}
else if (Message.Contains("help", StringComparison.OrdinalIgnoreCase))
{
    await Host.SendPrivateMessageAsync(
        Sender.Nickname.ToString(),
        "Available commands: !help, !info, !ping"
    );
}


// ========================================
// TEMPLATE 2: Channel Message Handler
// ========================================
// Event: ChannelMessageReceived
// Available variables:
//   - Sender: User      (the user who sent the message)
//   - Channel: Channel  (the channel where message was sent)
//   - Message: string   (the message content)
//   - Host: ScriptHost  (API for actions)

if (Message.StartsWith("!hello", StringComparison.OrdinalIgnoreCase))
{
    await Host.SendChannelMessageAsync(
        Channel.Name, 
        $"Hello {Sender.Nickname}! Welcome to {Channel.Name}!"
    );
}
else if (Message.StartsWith("!time", StringComparison.OrdinalIgnoreCase))
{
    var currentTime = DateTime.Now.ToString("HH:mm:ss");
    await Host.SendChannelMessageAsync(
        Channel.Name,
        $"Current time is: {currentTime}"
    );
}
else if (Message.StartsWith("!users", StringComparison.OrdinalIgnoreCase))
{
    var userCount = Channel.Users.Count;
    var userNames = string.Join(", ", Channel.Users.Select(u => u.Nickname.ToString()));
    await Host.SendChannelMessageAsync(
        Channel.Name,
        $"Users in channel ({userCount}): {userNames}"
    );
}


// ========================================
// TEMPLATE 3: User Joined Handler
// ========================================
// Event: UserJoinedChannel
// Available variables:
//   - User: User        (the user who joined)
//   - Channel: Channel  (the channel that was joined)
//   - Host: ScriptHost  (API for actions)

var nickname = User.Nickname.ToString();
var channelName = Channel.Name;

// Send welcome message
await Host.SendChannelMessageAsync(
    channelName, 
    $"Welcome to {channelName}, {nickname}! 🎉"
);

// Log the join event
Host.Log($"User {nickname} joined {channelName}");

// Optional: Send private welcome message
await Host.SendPrivateMessageAsync(
    nickname,
    $"Thanks for joining {channelName}! Type !help for available commands."
);


// ========================================
// TEMPLATE 4: User Left Handler
// ========================================
// Event: UserPartedChannel
// Available variables:
//   - User: User        (the user who parted)
//   - Channel: Channel  (the channel that was parted)
//   - Reason: string?   (optional reason, may be null)
//   - Host: ScriptHost  (API for actions)

var nickname = User.Nickname.ToString();
var channelName = Channel.Name;

// Create farewell message (with or without reason)
var farewellMessage = string.IsNullOrEmpty(Reason)
    ? $"Goodbye, {nickname}! See you soon! 👋"
    : $"Goodbye, {nickname}! ({Reason}) 👋";

// Send to channel
await Host.SendChannelMessageAsync(channelName, farewellMessage);

// Log the leave event
Host.Log($"User {nickname} left {channelName}" + 
    (string.IsNullOrEmpty(Reason) ? "" : $" (Reason: {Reason})"));


// ========================================
// ADVANCED EXAMPLES
// ========================================

// Example: Auto-kick spam
// Event: ChannelMessageReceived
/*
if (Message.Contains("spam", StringComparison.OrdinalIgnoreCase) ||
    Message.Contains("advertisement", StringComparison.OrdinalIgnoreCase))
{
    Host.Log($"Spam detected from {Sender.Nickname} in {Channel.Name}");
    // Note: Kick functionality would need to be added to ScriptHost
}
*/

// Example: Command with arguments
// Event: ChannelMessageReceived
/*
if (Message.StartsWith("!greet ", StringComparison.OrdinalIgnoreCase))
{
    var nameToGreet = Message.Substring(7).Trim();
    await Host.SendChannelMessageAsync(
        Channel.Name,
        $"{Sender.Nickname} says hello to {nameToGreet}!"
    );
}
*/

// Example: Timed auto-response
// Event: ChannelMessageReceived
/*
var lastResponseTime = DateTime.MinValue; // Store this somewhere persistent
if (Message.Contains("bot", StringComparison.OrdinalIgnoreCase))
{
    if ((DateTime.Now - lastResponseTime).TotalSeconds > 30)
    {
        await Host.SendChannelMessageAsync(
            Channel.Name,
            "Yes, I'm a bot! Type !help for commands."
        );
        lastResponseTime = DateTime.Now;
    }
}
*/

// Example: Role-based greeting
// Event: UserJoinedChannel
/*
var nickname = User.Nickname.ToString();
var channelName = Channel.Name;

// Check if user has operator status (nickname prefix)
var nicknameStr = (string)User.Nickname;
var isOperator = nicknameStr.StartsWith("@") || nicknameStr.StartsWith("~");

var greeting = isOperator
    ? $"Welcome back, operator {nickname}! 👑"
    : $"Welcome {nickname}! Enjoy your stay! 😊";

await Host.SendChannelMessageAsync(channelName, greeting);
*/


// ========================================
// TIPS AND BEST PRACTICES
// ========================================

/*
1. TYPE SAFETY:
   - All variables (except Reason in UserParted) are non-null
   - No need for null checks like Context.Message ?? ""
   - Compiler catches errors before runtime

2. EVENT SELECTION:
   - Each script responds to ONE event type
   - Select the appropriate event when creating the script
   - Multiple scripts can handle the same event type

3. PERFORMANCE:
   - Scripts run asynchronously (don't block IRC client)
   - Keep scripts simple and fast
   - Avoid infinite loops (30-second timeout)

4. AVAILABLE APIS:
   ScriptHost provides:
   - SendPrivateMessageAsync(nickname, message)
   - SendChannelMessageAsync(channelName, message)
   - JoinChannelAsync(channelName)
   - PartChannelAsync(channelName)
   - Log(message)
   - Client property (full IrcClient access)

5. TESTING:
   - Use "Edit in Visual Studio" for full IntelliSense
   - Test scripts in a development IRC server first
   - Check logs for errors

6. ORGANIZATION:
   - Name scripts descriptively (e.g., "WelcomeBot", "CommandHandler")
   - One responsibility per script
   - Combine related commands in one script

7. DEBUGGING:
   - Use Host.Log() to output debug information
   - Check console/logs for script errors
   - Compilation errors shown when creating script
*/
