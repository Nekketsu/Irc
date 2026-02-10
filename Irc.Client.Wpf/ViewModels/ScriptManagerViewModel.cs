using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Irc.Client.Scripting;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace Irc.Client.Wpf.ViewModels;

public partial class ScriptManagerViewModel : ObservableObject
{
    private readonly ScriptManager scriptManager;

    [ObservableProperty]
    private ObservableCollection<ScriptViewModel> scripts = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsEditing))]
    private ScriptViewModel selectedScript;

    [ObservableProperty]
    private string newScriptCode = string.Empty;

    [ObservableProperty]
    private string newScriptName = string.Empty;

    public bool IsEditing => SelectedScript is not null;

    public ScriptManagerViewModel()
    {
        // Design-time constructor
        scriptManager = null!;
    }

    public ScriptManagerViewModel(ScriptManager scriptManager)
    {
        this.scriptManager = scriptManager;
        this.scriptManager.ScriptsChanged += OnScriptsChanged;
        _ = InitializeAsync();
    }

    private void OnScriptsChanged(object? sender, EventArgs e)
    {
        // Refresh UI when scripts change (e.g., files added/removed externally)
        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            RefreshScripts();
        });
    }

    private async Task InitializeAsync()
    {
        // Only load scripts if not already loaded
        // ScriptManager may have already loaded scripts in IrcViewModel
        if (!scriptManager.Scripts.Any())
        {
            await scriptManager.LoadScriptsFromFolderAsync();
        }
        RefreshScripts();
    }

    [RelayCommand]
    private async Task LoadScriptFromFileAsync()
    {
        if (scriptManager is null) return;

        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "C# Script Files (*.csx)|*.csx|All Files (*.*)|*.*",
            Title = "Load Script"
        };

        if (dialog.ShowDialog() == true)
        {
            try
            {
                // Read the file
                var code = await File.ReadAllTextAsync(dialog.FileName);
                var fileName = Path.GetFileName(dialog.FileName);

                // Create script (will save to scripts folder)
                await scriptManager.CreateScriptAsync(code, fileName);
                RefreshScripts();

                MessageBox.Show($"Script '{fileName}' loaded successfully!",
                    "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading script: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    [RelayCommand]
    private async Task CreateScriptAsync()
    {
        if (scriptManager is null || string.IsNullOrWhiteSpace(NewScriptName) || string.IsNullOrWhiteSpace(NewScriptCode))
        {
            MessageBox.Show("Please enter both script name and code.",
                "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            await scriptManager.CreateScriptAsync(NewScriptCode, NewScriptName, overwrite: false);
            RefreshScripts();

            var savedName = NewScriptName;
            NewScriptCode = string.Empty;
            NewScriptName = string.Empty;

            MessageBox.Show($"Script '{savedName}' created successfully!",
                "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (ScriptAlreadyExistsException)
        {
            var result = MessageBox.Show(
                $"A script named '{NewScriptName}' already exists. Do you want to overwrite it?",
                "Script Exists",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await scriptManager.CreateScriptAsync(NewScriptCode, NewScriptName, overwrite: true);
                    RefreshScripts();

                    var savedName = NewScriptName;
                    NewScriptCode = string.Empty;
                    NewScriptName = string.Empty;

                    MessageBox.Show($"Script '{savedName}' overwritten successfully!",
                        "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error overwriting script: {ex.Message}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error creating script: {ex.Message}",
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private void EditScript(ScriptViewModel scriptVm)
    {
        if (scriptVm?.Script is null) return;

        // Load script into editor (use filename without extension for display)
        NewScriptName = Path.GetFileNameWithoutExtension(scriptVm.Name);
        NewScriptCode = scriptVm.Script.Code;
        SelectedScript = scriptVm;
    }

    [RelayCommand]
    private void DuplicateScript(ScriptViewModel scriptVm)
    {
        if (scriptVm?.Script is null) return;

        // Load script code into editor for creating a new script
        var baseName = Path.GetFileNameWithoutExtension(scriptVm.Name);

        // Generate a unique name
        var copyName = $"{baseName}_Copy";
        var counter = 1;
        while (Scripts.Any(s => Path.GetFileNameWithoutExtension(s.Name).Equals(copyName, StringComparison.OrdinalIgnoreCase)))
        {
            counter++;
            copyName = $"{baseName}_Copy{counter}";
        }

        NewScriptName = copyName;
        NewScriptCode = scriptVm.Script.Code;

        // Clear SelectedScript so it creates a new script instead of editing
        SelectedScript = null;

        MessageBox.Show($"Script code loaded. Edit and click 'Create Script' to save as '{copyName}.csx'.",
            "Copy Script", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    [RelayCommand]
    private async Task SaveScriptAsync()
    {
        if (scriptManager is null || SelectedScript?.Script is null)
        {
            MessageBox.Show("No script selected to save.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(NewScriptCode) || string.IsNullOrWhiteSpace(NewScriptName))
        {
            MessageBox.Show("Please provide both script name and code.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            // Update script (will save to disk)
            await scriptManager.UpdateScriptAsync(SelectedScript.Script.Name, NewScriptCode, NewScriptName);

            RefreshScripts();

            // Clear the editor
            var savedName = NewScriptName;
            NewScriptCode = string.Empty;
            NewScriptName = string.Empty;
            SelectedScript = null;

            MessageBox.Show($"Script '{savedName}' saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to save script: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private void CancelEdit()
    {
        // If editing an existing script, ask for confirmation
        if (SelectedScript is not null)
        {
            var result = MessageBox.Show(
                "Are you sure you want to discard your changes?",
                "Discard Changes",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }
        }

        // Clear the editor
        NewScriptCode = string.Empty;
        NewScriptName = string.Empty;
        SelectedScript = null;
    }

    [RelayCommand]
    private void ToggleScript(ScriptViewModel scriptVm)
    {
        if (scriptManager is null || scriptVm.Script is null) return;

        if (scriptVm.IsEnabled)
        {
            scriptManager.DisableScript(scriptVm.Script.Name);
        }
        else
        {
            scriptManager.EnableScript(scriptVm.Script.Name);
        }
        // No need to manually update IsEnabled - ScriptsChanged event will trigger RefreshScripts()
    }

    [RelayCommand]
    private void InsertTemplate()
    {
        // Insert a basic script template
        var template = @"// IRC Script Template
// Use Host.On* methods to subscribe to events

// Subscribe to private messages
Host.OnPrivateMessage(async args =>
{
    Host.Log($""Private message from {args.Sender.Nickname}: {args.Message}"");
    
    // Example: Auto-respond to private messages
    // await Host.SendPrivateMessageAsync(args.Sender.Nickname, ""Thanks for your message!"");
});

// Subscribe to channel messages
Host.OnChannelMessage(async args =>
{
    Host.Log($""Channel message in {args.Channel.Name} from {args.Sender.Nickname}: {args.Message}"");
    
    // Example: Respond to !help command
    if (args.Message == ""!help"")
    {
        await Host.SendChannelMessageAsync(args.Channel.Name, ""Available commands: !help, !about"");
    }
});

// Subscribe to user joined events
Host.OnUserJoined(async args =>
{
    Host.Log($""{args.User.Nickname} joined {args.Channel.Name}"");
    
    // Example: Greet new users
    // await Host.SendChannelMessageAsync(args.Channel.Name, $""Welcome {args.User.Nickname}!"");
});

// Subscribe to user parted events
Host.OnUserParted(async args =>
{
    Host.Log($""{args.User.Nickname} parted {args.Channel.Name}"");
});

Host.Log(""Script loaded successfully!"");
";
        NewScriptCode = template;
    }

    [RelayCommand]
    private void RemoveScript(ScriptViewModel scriptVm)
    {
        if (scriptManager is null || scriptVm.Script is null) return;

        var result = MessageBox.Show($"Are you sure you want to remove script '{scriptVm.Name}'?",
            "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            scriptManager.RemoveScript(scriptVm.Script.Name);
            RefreshScripts();
        }
    }

    private void RefreshScripts()
    {
        if (scriptManager is null) return;

        Scripts.Clear();
        // Order by name without extension
        var orderedScripts = scriptManager.Scripts
            .OrderBy(s => Path.GetFileNameWithoutExtension(s.Name), StringComparer.OrdinalIgnoreCase);
        foreach (var script in orderedScripts)
        {
            Scripts.Add(new ScriptViewModel(script));
        }
    }
}

public partial class ScriptViewModel : ObservableObject
{
    public Script Script { get; }

    [ObservableProperty]
    private string name;

    [ObservableProperty]
    private bool isEnabled;

    [ObservableProperty]
    private string eventTypesText;

    public ScriptViewModel(Script script)
    {
        Script = script;
        name = script.Name;
        isEnabled = script.IsEnabled;
        eventTypesText = script.SubscribedEvents.Count == 0
            ? "No events"
            : string.Join(", ", script.SubscribedEvents);
    }
}

