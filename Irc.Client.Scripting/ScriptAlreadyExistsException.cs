namespace Irc.Client.Scripting;

public class ScriptAlreadyExistsException : Exception
{
    public string FileName { get; }

    public ScriptAlreadyExistsException(string fileName)
        : base($"A script with the name '{fileName}' already exists.")
    {
        FileName = fileName;
    }
}
