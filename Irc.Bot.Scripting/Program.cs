using Irc.Bot.Scripting;
using Irc.Client;
using Irc.Client.Scripting;
using Irc.Messages.Messages;

Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
Console.WriteLine("║           IRC Bot with Script Management                  ║");
Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
Console.WriteLine();

// Configuration
Console.Write("Enter IRC server (default: localhost): ");
var server = Console.ReadLine();
if (string.IsNullOrWhiteSpace(server))
    server = "localhost";

Console.Write("Enter port (default: 6667): ");
var portInput = Console.ReadLine();
if (!int.TryParse(portInput, out int port))
    port = 6667;

Console.Write("Enter bot nickname (default: ScriptBot): ");
var nickname = Console.ReadLine();
if (string.IsNullOrWhiteSpace(nickname))
    nickname = "ScriptBot";

Console.Write("Enter channel to join (e.g., #test): ");
var channel = Console.ReadLine();
if (string.IsNullOrWhiteSpace(channel))
    channel = "#test";

Console.WriteLine();
Console.WriteLine($"Connecting to {server}:{port} as {nickname}...");

// Create IRC client and script manager
var client = new IrcClient(nickname, server, port);
var scriptManager = new ScriptManager(client);

// Create console command handler
var commandHandler = new ConsoleCommandHandler(client, scriptManager);
if (!string.IsNullOrWhiteSpace(channel))
{
    commandHandler.SetCurrentChannel(channel);
}

// Load scripts from Scripts folder
try
{
    Console.WriteLine("Loading scripts from Scripts folder...");
    await scriptManager.LoadScriptsFromFolderAsync();

    var enabledCount = scriptManager.Scripts.Count(s => s.IsEnabled);
    var disabledCount = scriptManager.Scripts.Count(s => !s.IsEnabled);

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"✓ Loaded {scriptManager.Scripts.Count} script(s)");
    Console.ResetColor();
    Console.WriteLine($"  - {enabledCount} enabled");
    Console.WriteLine($"  - {disabledCount} disabled");
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"⚠ Warning: Could not load scripts: {ex.Message}");
    Console.ResetColor();
}

Console.WriteLine();

// Show connection events
client.Connected += (sender, e) =>
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("[INFO] Connected to IRC server!");
    Console.ResetColor();
};

client.Disconnected += (sender, e) =>
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("[INFO] Disconnected from IRC server");
    Console.ResetColor();
};

// Show incoming messages
client.MessageReceived += (sender, message) =>
{
    if (message is PrivMsgMessage privMsg)
    {
        var sender_nick = privMsg.From.Split('!')[0];
        var target = privMsg.Target;

        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine($"[{target}] <{sender_nick}> {privMsg.Text}");
        Console.ResetColor();
    }
};

// Create cancellation token
using var cts = new CancellationTokenSource();

// Handle Ctrl+C
Console.CancelKeyPress += (sender, e) =>
{
    e.Cancel = true;
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("Disconnecting...");
    Console.ResetColor();
    cts.Cancel();
};

// Start client in background task
var clientTask = Task.Run(async () =>
{
    try
    {
        await client.RunAsync(cts.Token);
    }
    catch (OperationCanceledException)
    {
        // Expected on cancellation
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ERROR] Client error: {ex.Message}");
    }
});

// Wait a bit for connection
await Task.Delay(2000);

// Join the channel if connected
if (!cts.Token.IsCancellationRequested && !string.IsNullOrWhiteSpace(channel))
{
    Console.WriteLine($"Joining {channel}...");
    await client.SendMessageAsync(new JoinMessage(channel));
    await Task.Delay(1000);
}

Console.WriteLine();
Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
Console.WriteLine("║                    Bot is Running!                         ║");
Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
Console.WriteLine();

// Show active scripts
var activeScripts = scriptManager.Scripts.Where(s => s.IsEnabled).ToList();
if (activeScripts.Any())
{
    Console.WriteLine("Active Scripts:");
    foreach (var script in activeScripts)
    {
        var events = string.Join(", ", script.SubscribedEvents);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"  ✓ {script.Name,-30} [{events}]");
        Console.ResetColor();
    }
}
else
{
    Console.ForegroundColor = ConsoleColor.Gray;
    Console.WriteLine("  (No active scripts)");
    Console.ResetColor();
}

Console.WriteLine();
Console.WriteLine("Type /help for available commands");
Console.WriteLine("Type /quit to exit");
Console.WriteLine();
Console.WriteLine("─────────────────────────────────────────────────────────────");
Console.WriteLine();

// Command loop
while (!cts.Token.IsCancellationRequested)
{
    Console.Write("> ");
    var input = Console.ReadLine();

    if (string.IsNullOrEmpty(input))
        continue;

    if (!await commandHandler.HandleCommandAsync(input))
    {
        // User typed /quit
        cts.Cancel();
        break;
    }
}

Console.WriteLine();
Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine("Shutting down...");
Console.ResetColor();

// Cancel the client task
cts.Cancel();
await clientTask;

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("Bot stopped.");
Console.ResetColor();


