using System.Threading.Tasks;

namespace Irc.Messages.Messages
{
    public class PongMessage : Message
    {
        public string Server { get; set; }

        public PongMessage(string server)
        {
            var delimiter = ":";

            Server = (server.StartsWith(delimiter))
                ? server.Substring(delimiter.Length)
                : server;
        }

        public override string ToString()
        {
            return $"PONG :{Server}";
        }
    }
}