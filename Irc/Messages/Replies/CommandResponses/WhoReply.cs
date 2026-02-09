using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    [Command(RPL_WHOREPLY)]
    public class WhoReply : Reply
    {
        const string RPL_WHOREPLY = "352";
        public string ChannelName { get; set; }
        public string User { get; set; }
        public string Host { get; set; }
        public string Server { get; set; }
        public string Nickname { get; set; }
        public string RealName { get; set; }

        public WhoReply(string sender, string target, string channelName, string user, string host, string server, string nickname, string realName) : base(sender, target, RPL_WHOREPLY)
        {
            ChannelName = channelName;
            User = user;
            Host = host;
            Server = server;
            Nickname = nickname;
            RealName = realName;
        }

        public override string InnerToString()
        {
            return $"{ChannelName} {User} {Host} {Server} {Nickname} H :0 {RealName}";
        }

        public static new WhoReply Parse(string message)
        {
            var messageSplit = message.Split();

            var sender = message.Split(':').First();
            var target = messageSplit[2];
            var channelName = messageSplit[3];
            var user = messageSplit[4];
            var host = messageSplit[5];
            var server = messageSplit[6];
            var nickname = messageSplit[7];
            var realName = messageSplit.Last();

            return new(sender, target, channelName, user, host, server, nickname, realName);
        }
    }
}