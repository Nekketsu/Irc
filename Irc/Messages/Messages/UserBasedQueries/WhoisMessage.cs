using System;
using System.Linq;
using System.Threading.Tasks;
using Irc.Helpers;
using Messages.Replies.CommandResponses;
using Messages.Replies.ErrorReplies;

namespace Irc.Messages.Messages
{
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