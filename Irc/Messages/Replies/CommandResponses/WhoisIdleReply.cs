using System.Collections.Generic;
using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    public class WhoisIdleReply : Reply
    {
        public const string DefaultMessage = "seconds idle";

        const string RPL_WHOISIDLE = "312";
        public string Nickname { get; set; }
        public int Idle { get; set; }
        public string Message { get; set; }

        public WhoisIdleReply(string sender, string target, string nickname, int idle, string message) : base(sender, target, RPL_WHOISIDLE)
        {
            Nickname = nickname;
            Idle = idle;
            Message = message;
        }

        public override string InnerToString()
        {
            return $"{Nickname} {Idle} :{Message}";
        }
    }
}