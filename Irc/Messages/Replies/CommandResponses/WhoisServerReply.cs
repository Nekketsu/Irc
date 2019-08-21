using System.Collections.Generic;
using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    public class WhoisServerReply : Reply
    {
        const string RPL_WHOISSERVER = "312";
        public string Nickname { get; set; }
        public string Server { get; set; }
        public string ServerInfo { get; set; }

        public WhoisServerReply(string target, string nickname, string server, string serverInfo) : base(target, RPL_WHOISSERVER)
        {
            Nickname = nickname;
            Server = server;
            ServerInfo = serverInfo;
        }

        public override string InnerToString()
        {
            return $"{Nickname} {Server} :{ServerInfo}";
        }
    }
}