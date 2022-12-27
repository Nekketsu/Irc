namespace Irc.Messages.Messages
{
    [Command("PING")]
    public class PingMessage : Message
    {
        public string Server { get; set; }

        public PingMessage(string server)
        {
            Server = server;
        }

        public override string ToString()
        {
            return $"PING :{Server}";
        }
    }
}