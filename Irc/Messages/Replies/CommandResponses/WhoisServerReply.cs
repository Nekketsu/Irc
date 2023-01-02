using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    [Command(RPL_WHOISSERVER)]
    public class WhoisServerReply : Reply
    {
        const string RPL_WHOISSERVER = "312";
        public string Nickname { get; set; }
        public string Server { get; set; }
        public string ServerInfo { get; set; }

        public WhoisServerReply(string sender, string target, string nickname, string server, string serverInfo) : base(sender, target, RPL_WHOISSERVER)
        {
            Nickname = nickname;
            Server = server;
            ServerInfo = serverInfo;
        }

        public override string InnerToString()
        {
            return $"{Nickname} {Server} :{ServerInfo}";
        }

        public new static WhoisServerReply Parse(string message)
        {
            var messageSplit = message.Split();

            var sender = messageSplit[0];
            var target = messageSplit[2];
            var nickname = messageSplit[3];
            var server = messageSplit[4];
            var serverInfo = message
                .Substring(messageSplit[0].Length).TrimStart()
                .Substring(messageSplit[1].Length).TrimStart()
                .Substring(messageSplit[2].Length).TrimStart()
                .Substring(messageSplit[3].Length).TrimStart()
                .Substring(messageSplit[4].Length).TrimStart()
                .TrimStart(':');

            return new WhoisServerReply(sender, target, nickname, server, serverInfo);
        }
    }
}