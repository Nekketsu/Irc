namespace Irc.Messages
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandAttribute : Attribute
    {
        public string Command { get; set; }

        public CommandAttribute(string command)
        {
            Command = command;
        }
    }
}
