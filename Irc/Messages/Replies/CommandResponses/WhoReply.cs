using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    public class WhoReply : Reply
    {
        const string RPL_WHOREPLY = "352";
        public string ChannelName { get; set; }
        public string User { get; set; }
        public string Host { get; set; }
        public string Server { get; set; }
        public string NickName { get; set; }
        public string RealName { get; set; }

        public WhoReply(string target, string channelName, string user, string host, string server, string nickName, string realName) : base(target, RPL_WHOREPLY)
        {
            ChannelName = channelName;
            User = user;
            Host = host;
            Server = server;
            NickName = nickName;
            RealName = realName;
        }

        public override string InnerToString()
        {
            return $"{ChannelName} {User} {Host} {Server} {NickName} H :0 {RealName}";
        }
    }
}