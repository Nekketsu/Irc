using Irc.Client.Scripting.Api;
using System.Text.Json;

namespace Irc.Client.Scripting;

public class ScriptManager
{
    private IrcClient? client;
    private readonly ScriptEngine engine;
    private readonly List<ScriptInfo> scripts;
    private readonly string scriptsFolder;
    private readonly string manifestPath;
    private readonly FileSystemWatcher fileWatcher;
    private Action<string>? logAction;

    public IReadOnlyList<Script> Scripts => scripts.Select(s => s.Script).ToList().AsReadOnly();

    public event EventHandler? ScriptsChanged;

    public ScriptManager(IrcClient? client = null, string? scriptsFolder = null, Action<string>? logAction = null)
    {
        this.client = client;
        this.engine = new ScriptEngine();
        this.scripts = [];
        this.logAction = logAction;

        // Use default folder if not specified
        this.scriptsFolder = scriptsFolder ?? Path.Combine(
            AppContext.BaseDirectory,
            "Scripts");

        this.manifestPath = Path.Combine(this.scriptsFolder, "scripts.json");

        // Create folder if it doesn't exist
        Directory.CreateDirectory(this.scriptsFolder);

        // Setup file watcher for automatic synchronization
        fileWatcher = new FileSystemWatcher(this.scriptsFolder, "*.csx")
        {
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
            EnableRaisingEvents = true
        };

        fileWatcher.Created += (s, e) => _ = SynchronizeScriptsAsync();
        fileWatcher.Deleted += (s, e) => _ = SynchronizeScriptsAsync();
        fileWatcher.Changed += (s, e) => _ = SynchronizeScriptsAsync();
        fileWatcher.Renamed += (s, e) => _ = SynchronizeScriptsAsync();
    }

    public void SetClient(IrcClient client)
    {
        this.client = client;

        // Reload scripts to execute them with the new client
        // This will convert metadata-only scripts into fully functional ones
        _ = LoadScriptsFromFolderAsync();
    }

    private ScriptsManifest LoadManifest()
    {
        if (!File.Exists(manifestPath))
        {
            return new ScriptsManifest();
        }

        try
        {
            var json = File.ReadAllText(manifestPath);
            return JsonSerializer.Deserialize<ScriptsManifest>(json) ?? new ScriptsManifest();
        }
        catch
        {
            return new ScriptsManifest();
        }
    }

    private void SaveManifest(ScriptsManifest manifest)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        var json = JsonSerializer.Serialize(manifest, options);
        File.WriteAllText(manifestPath, json);
    }

    public async Task LoadScriptsFromFolderAsync()
    {
        if (!Directory.Exists(scriptsFolder))
            return;

        // Clear existing scripts to avoid duplicates
        foreach (var scriptInfo in scripts.ToList())
        {
            if (scriptInfo.Host is not null)
            {
                scriptInfo.Host.UnsubscribeAll();
            }
        }
        scripts.Clear();

        var manifest = LoadManifest();
        var scriptFiles = Directory.GetFiles(scriptsFolder, "*.csx");
        var fileNames = scriptFiles.Select(Path.GetFileName).ToHashSet();

        // Step 1: Remove manifest entries for scripts that don't exist on disk
        var orphanedMetadata = manifest.Scripts.Where(m => !fileNames.Contains(m.FileName)).ToList();
        foreach (var orphan in orphanedMetadata)
        {
            Console.WriteLine($"Removing orphaned metadata for: {orphan.FileName}");
            manifest.Scripts.Remove(orphan);
        }

        // Step 2: Add metadata for new scripts found on disk
        var metadataFileNames = manifest.Scripts.Select(m => m.FileName).ToHashSet();
        foreach (var filePath in scriptFiles)
        {
            var fileName = Path.GetFileName(filePath);
            if (!metadataFileNames.Contains(fileName))
            {
                Console.WriteLine($"New script detected: {fileName}, adding as disabled");
                manifest.Scripts.Add(new ScriptMetadata
                {
                    FileName = fileName,
                    IsEnabled = false, // New scripts default to disabled
                    CreatedAt = File.GetCreationTimeUtc(filePath),
                    LastModified = File.GetLastWriteTimeUtc(filePath)
                });
            }
        }

        // Step 3: Load all scripts and apply their metadata
        foreach (var filePath in scriptFiles)
        {
            try
            {
                var fileName = Path.GetFileName(filePath);
                var metadata = manifest.Scripts.First(m => m.FileName == fileName);

                if (client is not null)
                {
                    // Load and execute the script if client is available
                    var script = await LoadScriptInternalAsync(filePath);

                    // Apply enabled/disabled state from metadata
                    script.IsEnabled = metadata.IsEnabled;

                    // If disabled, unsubscribe from events
                    if (!script.IsEnabled)
                    {
                        var scriptInfo = scripts.FirstOrDefault(s => s.Script.Name.Equals(script.Name, StringComparison.OrdinalIgnoreCase));
                        if (scriptInfo?.Host is not null)
                        {
                            scriptInfo.Host.UnsubscribeAll();
                        }
                    }
                }
                else
                {
                    // Just load script metadata without executing them
                    var code = await File.ReadAllTextAsync(filePath);
                    var script = new Script(fileName, code)
                    {
                        FilePath = filePath,
                        IsEnabled = metadata.IsEnabled
                    };

                    scripts.Add(new ScriptInfo
                    {
                        Script = script,
                        Host = null! // No host when client is not available
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load script {filePath}: {ex.Message}");
            }
        }

        // Step 4: Save the synchronized manifest
        SaveManifest(manifest);
    }

    public async Task<Script> CreateScriptAsync(string code, string fileName, bool overwrite = false)
    {
        // Ensure .csx extension
        if (!fileName.EndsWith(".csx", StringComparison.OrdinalIgnoreCase))
        {
            fileName += ".csx";
        }

        var filePath = Path.Combine(scriptsFolder, fileName);

        // Check if script already exists
        if (File.Exists(filePath) && !overwrite)
        {
            throw new ScriptAlreadyExistsException(fileName);
        }

        // If overwriting, remove the old script from memory
        if (overwrite)
        {
            var existingScript = scripts.FirstOrDefault(s => s.Script.Name.Equals(fileName, StringComparison.OrdinalIgnoreCase));
            if (existingScript is not null)
            {
                if (existingScript.Host is not null)
                {
                    existingScript.Host.UnsubscribeAll();
                }
                scripts.Remove(existingScript);
            }
        }

        // Save to disk
        await File.WriteAllTextAsync(filePath, code);

        // Update manifest
        var manifest = LoadManifest();
        var existing = manifest.Scripts.FirstOrDefault(m => m.FileName == fileName);

        if (existing is not null)
        {
            existing.LastModified = DateTime.UtcNow;
            existing.IsEnabled = true;
        }
        else
        {
            manifest.Scripts.Add(new ScriptMetadata
            {
                FileName = fileName,
                IsEnabled = true,
                CreatedAt = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            });
        }

        SaveManifest(manifest);

        // Create script object (metadata only if no client)
        Script script;
        if (client is not null)
        {
            // Load and execute script if client is available
            script = await LoadScriptInternalAsync(filePath);
        }
        else
        {
            // Create metadata-only script if no client
            script = new Script(fileName, code) { FilePath = filePath, IsEnabled = true };
            scripts.Add(new ScriptInfo
            {
                Script = script,
                Host = null!
            });
        }

        ScriptsChanged?.Invoke(this, EventArgs.Empty);

        return script;
    }

    private async Task<Script> LoadScriptInternalAsync(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Script file not found: {filePath}");
        }

        if (client is null)
        {
            throw new InvalidOperationException("Cannot load scripts without an IRC client. Connect to a server first.");
        }

        var code = await File.ReadAllTextAsync(filePath);
        var fileName = Path.GetFileName(filePath);

        var script = new Script(fileName, code) { FilePath = filePath };
        var host = new ScriptHost(client, logAction);

        var result = await engine.ExecuteScriptAsync(code, host);

        if (!result.IsSuccess)
        {
            throw new InvalidOperationException($"Failed to execute script: {result.ErrorMessage}");
        }

        // Get subscribed event types from host
        var subscribedEvents = host.GetSubscribedEventTypes();
        script.SubscribedEvents = subscribedEvents;

        scripts.Add(new ScriptInfo
        {
            Script = script,
            Host = host
        });

        return script;
    }

    public async Task<Script> LoadScriptAsync(string filePath)
    {
        var script = await LoadScriptInternalAsync(filePath);

        // Update manifest
        var manifest = LoadManifest();
        var fileName = Path.GetFileName(filePath);

        if (!manifest.Scripts.Any(m => m.FileName == fileName))
        {
            manifest.Scripts.Add(new ScriptMetadata
            {
                FileName = fileName,
                IsEnabled = true,
                CreatedAt = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            });
            SaveManifest(manifest);
        }

        return script;
    }

    public async Task UpdateScriptAsync(string scriptName, string code, string fileName)
    {
        var scriptInfo = scripts.FirstOrDefault(s => s.Script.Name.Equals(scriptName, StringComparison.OrdinalIgnoreCase));
        if (scriptInfo is null)
        {
            throw new InvalidOperationException("Script not found");
        }

        // Ensure .csx extension
        if (!fileName.EndsWith(".csx", StringComparison.OrdinalIgnoreCase))
        {
            fileName += ".csx";
        }

        var oldFilePath = scriptInfo.Script.FilePath;
        var oldFileName = Path.GetFileName(oldFilePath);
        var newFilePath = Path.Combine(scriptsFolder, fileName);

        // Save to new file
        await File.WriteAllTextAsync(newFilePath, code);

        // Remove old script
        if (scriptInfo.Host is not null)
        {
            scriptInfo.Host.UnsubscribeAll();
        }
        scripts.Remove(scriptInfo);

        // Delete old file if path changed
        if (oldFilePath != newFilePath && File.Exists(oldFilePath))
        {
            File.Delete(oldFilePath);
        }

        // Update manifest
        var manifest = LoadManifest();

        // Remove old entry
        manifest.Scripts.RemoveAll(m => m.FileName == oldFileName);

        // Add/update new entry
        var existing = manifest.Scripts.FirstOrDefault(m => m.FileName == fileName);
        if (existing is not null)
        {
            existing.LastModified = DateTime.UtcNow;
        }
        else
        {
            manifest.Scripts.Add(new ScriptMetadata
            {
                FileName = fileName,
                IsEnabled = true,
                CreatedAt = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            });
        }

        SaveManifest(manifest);

        // Create new script (load only if client is available)
        if (client is not null)
        {
            await LoadScriptInternalAsync(newFilePath);
        }
        else
        {
            // Create metadata-only script if no client
            var script = new Script(fileName, code) { FilePath = newFilePath, IsEnabled = true };
            scripts.Add(new ScriptInfo
            {
                Script = script,
                Host = null!
            });
        }

        ScriptsChanged?.Invoke(this, EventArgs.Empty);
    }

    public void RemoveScript(string scriptName)
    {
        var scriptInfo = scripts.FirstOrDefault(s => s.Script.Name.Equals(scriptName, StringComparison.OrdinalIgnoreCase));
        if (scriptInfo is not null)
        {
            // Unsubscribe from all events
            if (scriptInfo.Host is not null)
            {
                scriptInfo.Host.UnsubscribeAll();
            }
            scripts.Remove(scriptInfo);

            var fileName = Path.GetFileName(scriptInfo.Script.FilePath);

            // Delete file
            if (File.Exists(scriptInfo.Script.FilePath))
            {
                File.Delete(scriptInfo.Script.FilePath);
            }

            // Update manifest
            var manifest = LoadManifest();
            manifest.Scripts.RemoveAll(m => m.FileName == fileName);
            SaveManifest(manifest);

            ScriptsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public void EnableScript(string scriptName)
    {
        var scriptInfo = scripts.FirstOrDefault(s => s.Script.Name.Equals(scriptName, StringComparison.OrdinalIgnoreCase));
        if (scriptInfo is not null && !scriptInfo.Script.IsEnabled)
        {
            scriptInfo.Script.IsEnabled = true;

            // Re-subscribe to events (only if Host is available)
            if (scriptInfo.Host is not null)
            {
                scriptInfo.Host.UnsubscribeAll(); // Clean slate
                _ = engine.ExecuteScriptAsync(scriptInfo.Script.Code, scriptInfo.Host); // Re-execute to subscribe
            }

            // Update manifest
            var manifest = LoadManifest();
            var fileName = Path.GetFileName(scriptInfo.Script.FilePath);
            var metadata = manifest.Scripts.FirstOrDefault(m => m.FileName == fileName);

            if (metadata is not null)
            {
                metadata.IsEnabled = true;
                metadata.LastModified = DateTime.UtcNow;
                SaveManifest(manifest);
            }

            ScriptsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public void DisableScript(string scriptName)
    {
        var scriptInfo = scripts.FirstOrDefault(s => s.Script.Name.Equals(scriptName, StringComparison.OrdinalIgnoreCase));
        if (scriptInfo is not null && scriptInfo.Script.IsEnabled)
        {
            scriptInfo.Script.IsEnabled = false;

            // Unsubscribe from all events (only if Host is available)
            if (scriptInfo.Host is not null)
            {
                scriptInfo.Host.UnsubscribeAll();
            }

            // Update manifest
            var manifest = LoadManifest();
            var fileName = Path.GetFileName(scriptInfo.Script.FilePath);
            var metadata = manifest.Scripts.FirstOrDefault(m => m.FileName == fileName);

            if (metadata is not null)
            {
                metadata.IsEnabled = false;
                metadata.LastModified = DateTime.UtcNow;
                SaveManifest(manifest);
            }

            ScriptsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private async Task SynchronizeScriptsAsync()
    {
        try
        {
            if (!Directory.Exists(scriptsFolder))
                return;

            var manifest = LoadManifest();
            var scriptFiles = Directory.GetFiles(scriptsFolder, "*.csx");
            var fileNames = scriptFiles.Select(Path.GetFileName).ToHashSet();
            var currentScriptFiles = scripts.Select(s => Path.GetFileName(s.Script.FilePath)).ToHashSet();
            var hasChanges = false;

            // Step 1: Remove scripts that no longer exist on disk
            var scriptsToRemove = scripts.Where(s => !File.Exists(s.Script.FilePath)).ToList();
            foreach (var scriptInfo in scriptsToRemove)
            {
                scriptInfo.Host.UnsubscribeAll();
                scripts.Remove(scriptInfo);

                var fileName = Path.GetFileName(scriptInfo.Script.FilePath);
                manifest.Scripts.RemoveAll(m => m.FileName == fileName);
                hasChanges = true;
                Console.WriteLine($"Removed deleted script: {fileName}");
            }

            // Step 2: Add metadata for new scripts found on disk
            var metadataFileNames = manifest.Scripts.Select(m => m.FileName).ToHashSet();
            foreach (var filePath in scriptFiles)
            {
                var fileName = Path.GetFileName(filePath);

                // New script detected - add to manifest as disabled
                if (!metadataFileNames.Contains(fileName))
                {
                    manifest.Scripts.Add(new ScriptMetadata
                    {
                        FileName = fileName,
                        IsEnabled = false,
                        CreatedAt = File.GetCreationTimeUtc(filePath),
                        LastModified = File.GetLastWriteTimeUtc(filePath)
                    });
                    hasChanges = true;
                    Console.WriteLine($"New script detected: {fileName}, added as disabled");
                }

                // Load new script if not already loaded (only if client is available)
                if (!currentScriptFiles.Contains(fileName) && client is not null)
                {
                    try
                    {
                        var script = await LoadScriptInternalAsync(filePath);
                        var metadata = manifest.Scripts.First(m => m.FileName == fileName);
                        script.IsEnabled = metadata.IsEnabled;

                        // If disabled, unsubscribe
                        if (!script.IsEnabled)
                        {
                            var scriptInfo = scripts.FirstOrDefault(s => s.Script.Name.Equals(script.Name, StringComparison.OrdinalIgnoreCase));
                            if (scriptInfo?.Host is not null)
                            {
                                scriptInfo.Host.UnsubscribeAll();
                            }
                        }

                        hasChanges = true;
                        Console.WriteLine($"Loaded new script: {fileName}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to load new script {fileName}: {ex.Message}");
                    }
                }
            }

            // Step 3: Save manifest if changes occurred
            if (hasChanges)
            {
                SaveManifest(manifest);
                ScriptsChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error synchronizing scripts: {ex.Message}");
        }
    }

    private class ScriptInfo
    {
        public Script Script { get; set; } = null!;
        public ScriptHost Host { get; set; } = null!;
    }
}
