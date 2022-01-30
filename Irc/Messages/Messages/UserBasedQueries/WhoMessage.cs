namespace Irc.Messages.Messages
{
    public class WhoMessage : Message
    {
        public string Mask { get; set; }

        public WhoMessage(string mask = null)
        {
            Mask = mask;
        }

        public override string ToString()
        {
            return $"{Command} {Mask}";
        }
    }
}