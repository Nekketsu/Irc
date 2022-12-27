namespace Irc.Messages.Messages
{
    [Command("LIST")]
    public class ListMessage : Message
    {
        public ListMessage()
        {
        }

        public override string ToString()
        {
            return $"{Command}";
        }
    }
}