namespace Irc.Messages.Messages
{
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