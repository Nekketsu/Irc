using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Irc.Helpers;
using Irc.Messages;
using Messages.Replies.CommandResponses;

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