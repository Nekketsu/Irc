namespace Irc.Messages.Messages
{
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