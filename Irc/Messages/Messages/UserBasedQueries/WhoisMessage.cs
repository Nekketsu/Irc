namespace Irc.Messages.Messages
{
    [Command("WHOIS")]
    public class WhoisMessage : Message
    {
        public string Mask { get; set; }

        public WhoisMessage(string mask = null)
        {
            Mask = mask;
        }

        public override string ToString()
        {
            return $"{Command} {Mask}";
        }
    }
}