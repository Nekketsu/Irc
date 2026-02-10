using System.Text.Json.Serialization;

namespace Irc.Client.Scripting;

/// <summary>
/// Metadata for a script that is persisted to disk.
/// </summary>
public class ScriptMetadata
{
    /// <summary>
    /// The filename of the script (e.g., "AutoResponder.csx").
    /// </summary>
    [JsonPropertyName("fileName")]
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Whether the script is enabled or disabled.
    /// </summary>
    [JsonPropertyName("isEnabled")]
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Optional description of what the script does.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// When the script was created.
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When the script was last modified.
    /// </summary>
    [JsonPropertyName("lastModified")]
    public DateTime LastModified { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Container for all script metadata.
/// </summary>
public class ScriptsManifest
{
    [JsonPropertyName("scripts")]
    public List<ScriptMetadata> Scripts { get; set; } = [];
}
