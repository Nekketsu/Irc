namespace Irc.Client.Scripting
{
    /// <summary>
    /// Result of a script command execution.
    /// </summary>
    public class ScriptCommandResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; }

        public static ScriptCommandResult Ok(string message, object? data = null)
        {
            return new ScriptCommandResult
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static ScriptCommandResult Error(string message)
        {
            return new ScriptCommandResult
            {
                Success = false,
                Message = message
            };
        }
    }

    /// <summary>
    /// Handles script management commands.
    /// Shared between console bot and WPF client.
    /// </summary>
    public class ScriptCommandHandler
    {
        private readonly ScriptManager scriptManager;

        public ScriptCommandHandler(ScriptManager scriptManager)
        {
            this.scriptManager = scriptManager;
        }

        // Helper method to get script by index or name
        private Script? GetScriptByIndexOrName(string identifier)
        {
            // Order by name without extension
            var scripts = scriptManager.Scripts
                .OrderBy(s => Path.GetFileNameWithoutExtension(s.Name), StringComparer.OrdinalIgnoreCase)
                .ToList();

            // Try to parse as index (1-based)
            if (int.TryParse(identifier, out var index))
            {
                if (index >= 1 && index <= scripts.Count)
                {
                    return scripts[index - 1];
                }
                return null;
            }

            // Try to find by exact name (with or without extension)
            var script = scripts.FirstOrDefault(s => s.Name.Equals(identifier, StringComparison.OrdinalIgnoreCase));

            if (script is not null)
            {
                return script;
            }

            // Try adding .csx extension if not present
            if (!identifier.EndsWith(".csx", StringComparison.OrdinalIgnoreCase))
            {
                var nameWithExtension = identifier + ".csx";
                script = scripts.FirstOrDefault(s => s.Name.Equals(nameWithExtension, StringComparison.OrdinalIgnoreCase));
            }

            return script;
        }

        /// <summary>
        /// Executes a script command and returns the result.
        /// </summary>
        /// <param name="commandLine">Command line starting with subcommand (e.g., "list" or "add https://...")</param>
        public async Task<ScriptCommandResult> ExecuteAsync(string commandLine)
        {
            if (string.IsNullOrWhiteSpace(commandLine))
            {
                return ScriptCommandResult.Error("Usage: script <subcommand>\nSubcommands: list, add, remove, enable, disable, reload, show");
            }

            var parts = commandLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
            {
                return ScriptCommandResult.Error("Usage: script <subcommand>\nSubcommands: list, add, remove, enable, disable, reload, show");
            }

            var subcommand = parts[0].ToLower();

            try
            {
                switch (subcommand)
                {
                    case "list":
                    case "ls":
                        return await ListScriptsAsync();

                    case "add":
                        return await AddScriptAsync(parts);

                    case "remove":
                    case "rm":
                    case "delete":
                        return await RemoveScriptAsync(parts);

                    case "enable":
                        return await EnableScriptAsync(parts);

                    case "disable":
                        return await DisableScriptAsync(parts);

                    case "reload":
                        return await ReloadScriptsAsync();

                    case "show":
                    case "view":
                    case "cat":
                        return await ShowScriptAsync(parts);

                    default:
                        return ScriptCommandResult.Error($"Unknown subcommand: {subcommand}\nAvailable: list, add, remove, enable, disable, reload, show");
                }
            }
            catch (Exception ex)
            {
                return ScriptCommandResult.Error($"Error executing command: {ex.Message}");
            }
        }

        private async Task<ScriptCommandResult> ListScriptsAsync()
        {
            // Ensure scripts are loaded
            if (!scriptManager.Scripts.Any())
            {
                await scriptManager.LoadScriptsFromFolderAsync();
            }

            var scripts = scriptManager.Scripts;

            if (!scripts.Any())
            {
                return ScriptCommandResult.Ok("No scripts found.", scripts);
            }

            var enabledCount = scripts.Count(s => s.IsEnabled);
            var disabledCount = scripts.Count(s => !s.IsEnabled);

            // Build detailed list with index
            var lines = new List<string>();
            lines.Add($"Scripts ({scripts.Count} total: {enabledCount} enabled, {disabledCount} disabled):");
            lines.Add("");

            var orderedScripts = scripts
                .OrderBy(s => Path.GetFileNameWithoutExtension(s.Name), StringComparer.OrdinalIgnoreCase)
                .ToList();
            for (int i = 0; i < orderedScripts.Count; i++)
            {
                var script = orderedScripts[i];
                var index = i + 1;
                var status = script.IsEnabled ? "[✓]" : "[ ]";
                var eventInfo = script.SubscribedEvents.Any()
                    ? $" - Events: {string.Join(", ", script.SubscribedEvents)}"
                    : "";
                lines.Add($"  {index}. {status} {script.Name}{eventInfo}");
            }

            var message = string.Join("\n", lines);
            return ScriptCommandResult.Ok(message, scripts);
        }

        private async Task<ScriptCommandResult> AddScriptAsync(string[] parts)
        {
            if (parts.Length < 2)
            {
                return ScriptCommandResult.Error("Usage: script add <url-or-path>\nExample: script add https://example.com/script.csx\nExample: script add C:\\path\\to\\script.csx");
            }

            var source = parts[1];
            string code;
            string fileName;

            try
            {
                // Check if it's a URL
                if (source.StartsWith("http://") || source.StartsWith("https://"))
                {
                    using var httpClient = new HttpClient();
                    code = await httpClient.GetStringAsync(source);
                    fileName = Path.GetFileName(new Uri(source).LocalPath);

                    if (!fileName.EndsWith(".csx"))
                    {
                        fileName += ".csx";
                    }
                }
                else
                {
                    // Assume it's a local file path
                    if (!File.Exists(source))
                    {
                        return ScriptCommandResult.Error($"File not found: {source}");
                    }

                    code = await File.ReadAllTextAsync(source);
                    fileName = Path.GetFileName(source);
                }

                var script = await scriptManager.CreateScriptAsync(code, fileName, overwrite: false);
                return ScriptCommandResult.Ok($"Script '{script.Name}' added and enabled successfully.", script);
            }
            catch (ScriptAlreadyExistsException ex)
            {
                return ScriptCommandResult.Error($"Script '{ex.FileName}' already exists. Remove it first or rename the new script.");
            }
        }

        private async Task<ScriptCommandResult> RemoveScriptAsync(string[] parts)
        {
            if (parts.Length < 2)
            {
                return ScriptCommandResult.Error("Usage: script remove <index-or-name>\nExample: script remove 1\nExample: script remove AutoResponder");
            }

            var identifier = parts[1];
            var script = GetScriptByIndexOrName(identifier);

            if (script is null)
            {
                return ScriptCommandResult.Error($"Script not found: {identifier}");
            }

            scriptManager.RemoveScript(script.Name);
            return ScriptCommandResult.Ok($"Script '{script.Name}' removed.", script);
        }

        private async Task<ScriptCommandResult> EnableScriptAsync(string[] parts)
        {
            if (parts.Length < 2)
            {
                return ScriptCommandResult.Error("Usage: script enable <index-or-name>\nExample: script enable 1\nExample: script enable Welcome");
            }

            var identifier = parts[1];
            var script = GetScriptByIndexOrName(identifier);

            if (script is null)
            {
                return ScriptCommandResult.Error($"Script not found: {identifier}");
            }

            if (script.IsEnabled)
            {
                return ScriptCommandResult.Error($"Script '{script.Name}' is already enabled.");
            }

            scriptManager.EnableScript(script.Name);
            return ScriptCommandResult.Ok($"Script '{script.Name}' enabled.", script);
        }

        private async Task<ScriptCommandResult> DisableScriptAsync(string[] parts)
        {
            if (parts.Length < 2)
            {
                return ScriptCommandResult.Error("Usage: script disable <index-or-name>\nExample: script disable 1\nExample: script disable Welcome");
            }

            var identifier = parts[1];
            var script = GetScriptByIndexOrName(identifier);

            if (script is null)
            {
                return ScriptCommandResult.Error($"Script not found: {identifier}");
            }

            if (!script.IsEnabled)
            {
                return ScriptCommandResult.Error($"Script '{script.Name}' is already disabled.");
            }

            scriptManager.DisableScript(script.Name);
            return ScriptCommandResult.Ok($"Script '{script.Name}' disabled.", script);
        }

        private async Task<ScriptCommandResult> ReloadScriptsAsync()
        {
            await scriptManager.LoadScriptsFromFolderAsync();
            var scripts = scriptManager.Scripts;
            var enabledCount = scripts.Count(s => s.IsEnabled);
            var disabledCount = scripts.Count(s => !s.IsEnabled);

            return ScriptCommandResult.Ok($"Scripts reloaded: {enabledCount} enabled, {disabledCount} disabled", scripts);
        }

        private async Task<ScriptCommandResult> ShowScriptAsync(string[] parts)
        {
            if (parts.Length < 2)
            {
                return ScriptCommandResult.Error("Usage: script show <script-name-or-index>");
            }

            var scriptIdentifier = parts[1];
            var script = GetScriptByIndexOrName(scriptIdentifier);

            if (script is null)
            {
                return ScriptCommandResult.Error($"Script '{scriptIdentifier}' not found.");
            }

            if (string.IsNullOrEmpty(script.FilePath) || !File.Exists(script.FilePath))
            {
                return ScriptCommandResult.Error($"Script file not found: {script.Name}");
            }

            var content = await File.ReadAllTextAsync(script.FilePath);
            var lines = content.Split('\n');
            var numberedContent = string.Join("\n",
                lines.Select((line, index) => $"{index + 1,4}: {line}"));

            return ScriptCommandResult.Ok($"Script: {script.Name}\n{numberedContent}", content);
        }
    }
}
