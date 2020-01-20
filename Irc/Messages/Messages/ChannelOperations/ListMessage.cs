using System.Threading.Tasks;
using Messages.Replies.CommandResponses;

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