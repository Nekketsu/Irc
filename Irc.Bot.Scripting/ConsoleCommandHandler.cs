using Irc.Client;
using Irc.Client.Scripting;
using Irc.Messages.Messages;

namespace Irc.Bot.Scripting
{
    /// <summary>
    /// Handles console commands typed directly into the bot.
    /// </summary>
    public class ConsoleCommandHandler
    {
        private readonly IrcClient client;
        private readonly ScriptCommandHandler scriptCommandHandler;
        private string currentChannel = string.Empty;

        public ConsoleCommandHandler(IrcClient client, ScriptManager scriptManager)
        {
            this.client = client;
            this.scriptCommandHandler = new ScriptCommandHandler(scriptManager);
        }

        public void SetCurrentChannel(string channel)
        {
            currentChannel = channel;
        }

        public async Task<bool> HandleCommandAsync(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return true;

            // Must start with /
            if (!input.StartsWith("/"))
            {
                Console.WriteLine("Commands must start with /. Type /help for available commands.");
                return true;
            }

            var parts = input.Substring(1).Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
                return true;

            var command = parts[0].ToLower();

            try
            {
                switch (command)
                {
                    case "script":
                        await HandleScriptCommandAsync(parts);
                        break;

                    case "join":
                        await HandleJoinCommandAsync(parts);
                        break;

                    case "part":
                    case "leave":
                        await HandlePartCommandAsync(parts);
                        break;

                    case "msg":
                        await HandleMsgCommandAsync(parts);
                        break;

                    case "quit":
                    case "exit":
                        return false; // Signal to exit

                    case "help":
                        ShowHelp();
                        break;

                    default:
                        Console.WriteLine($"Unknown command: /{command}. Type /help for available commands.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing command: {ex.Message}");
            }

            return true;
        }

        private async Task HandleScriptCommandAsync(string[] parts)
        {
            // Remove "script" prefix and pass the rest to shared handler
            var commandLine = string.Join(" ", parts.Skip(1));
            var result = await scriptCommandHandler.ExecuteAsync(commandLine);

            if (result.Success)
            {
                // Handle list command specially to show formatted output
                if (parts.Length > 1 && (parts[1].ToLower() == "list" || parts[1].ToLower() == "ls"))
                {
                    await HandleScriptListAsync();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"✓ {result.Message}");
                    Console.ResetColor();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ {result.Message}");
                Console.ResetColor();
            }
        }

        private async Task HandleScriptListAsync()
        {
            var result = await scriptCommandHandler.ExecuteAsync("list");

            if (!result.Success)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ {result.Message}");
                Console.ResetColor();
                return;
            }

            var scripts = result.Data as IReadOnlyList<Script>;
            if (scripts is null || !scripts.Any())
            {
                Console.WriteLine("No scripts loaded.");
                return;
            }

            Console.WriteLine($"\nLoaded scripts ({scripts.Count}):");
            Console.WriteLine("─────────────────────────────────────────────────────────");

            foreach (var script in scripts)
            {
                var status = script.IsEnabled ? "✓ Enabled " : "✗ Disabled";
                var events = string.Join(", ", script.SubscribedEvents);
                var eventsText = string.IsNullOrEmpty(events) ? "(no events)" : $"[{events}]";

                Console.ForegroundColor = script.IsEnabled ? ConsoleColor.Green : ConsoleColor.Gray;
                Console.WriteLine($"  {status,-12} {script.Name,-30} {eventsText}");
                Console.ResetColor();
            }

            Console.WriteLine("─────────────────────────────────────────────────────────");
            Console.WriteLine($"Total: {scripts.Count(s => s.IsEnabled)} enabled, {scripts.Count(s => !s.IsEnabled)} disabled\n");
        }

        private async Task HandleJoinCommandAsync(string[] parts)
        {
            if (parts.Length < 2)
            {
                Console.WriteLine("Usage: /join <channel>");
                Console.WriteLine("Example: /join #test");
                return;
            }

            var channel = parts[1];
            if (!channel.StartsWith("#"))
            {
                channel = "#" + channel;
            }

            await client.SendMessageAsync(new JoinMessage(channel));
            currentChannel = channel;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✓ Joining {channel}...");
            Console.ResetColor();
        }

        private async Task HandlePartCommandAsync(string[] parts)
        {
            string channel;

            if (parts.Length >= 2)
            {
                channel = parts[1];
            }
            else if (!string.IsNullOrEmpty(currentChannel))
            {
                channel = currentChannel;
            }
            else
            {
                Console.WriteLine("Usage: /part [channel]");
                Console.WriteLine("Example: /part #test");
                return;
            }

            if (!channel.StartsWith("#"))
            {
                channel = "#" + channel;
            }

            await client.SendMessageAsync(new PartMessage(channel));

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✓ Leaving {channel}...");
            Console.ResetColor();

            if (currentChannel == channel)
            {
                currentChannel = string.Empty;
            }
        }

        private async Task HandleMsgCommandAsync(string[] parts)
        {
            if (parts.Length < 3)
            {
                Console.WriteLine("Usage: /msg <target> <message>");
                Console.WriteLine("Example: /msg #test Hello everyone!");
                Console.WriteLine("Example: /msg Alice Hi there!");
                return;
            }

            var target = parts[1];
            var message = string.Join(" ", parts.Skip(2));

            await client.SendMessageAsync(new PrivMsgMessage(target, message));

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"→ [{target}] {message}");
            Console.ResetColor();
        }

        private void ShowHelp()
        {
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    Available Commands                      ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine("Script Management:");
            Console.WriteLine("  /script list                - List all scripts");
            Console.WriteLine("  /script add <url-or-path>   - Add a new script");
            Console.WriteLine("  /script remove <name>       - Remove a script");
            Console.WriteLine("  /script enable <name>       - Enable a script");
            Console.WriteLine("  /script disable <name>      - Disable a script");
            Console.WriteLine("  /script reload              - Reload scripts from folder");
            Console.WriteLine();
            Console.WriteLine("IRC Commands:");
            Console.WriteLine("  /join <channel>             - Join a channel");
            Console.WriteLine("  /part [channel]             - Leave a channel");
            Console.WriteLine("  /msg <target> <message>     - Send a message");
            Console.WriteLine();
            Console.WriteLine("Bot Control:");
            Console.WriteLine("  /help                       - Show this help");
            Console.WriteLine("  /quit                       - Exit the bot");
            Console.WriteLine();
        }
    }
}
