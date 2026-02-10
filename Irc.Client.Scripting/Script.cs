namespace Irc.Client.Scripting;

public class Script
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Event type names this script is subscribed to (determined after execution).
    /// </summary>
    public List<string> SubscribedEvents { get; internal set; } = [];

    public Script(string name, string code)
    {
        Name = name;
        Code = code;
    }

    public static async Task<Script> LoadFromFileAsync(string filePath)
    {
        var name = Path.GetFileName(filePath);
        var code = await File.ReadAllTextAsync(filePath);

        return new Script(name, code) { FilePath = filePath };
    }
}
