using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    [Command(RPL_YOURHOST)]
    public class YourHostReply : Reply
    {
        const string RPL_YOURHOST = "002";

        public string ServerName { get;  }
        public string Version { get; }

        public YourHostReply(string sender, string target, string serverName, string version) : base(sender, target, RPL_YOURHOST)
        {
            ServerName = serverName;
            Version = version;
        }

        public override string InnerToString()
        {
            return $":Your host is {ServerName}, running version {Version}";
        }

        public new static WelcomeReply Parse(string message)
        {
            var messageSplit = message.Split();

            var sender = messageSplit[0];
            var target = messageSplit[2];
            var serverName = message
                .Substring(messageSplit[0].Length).TrimStart()
                .Substring(messageSplit[1].Length).TrimStart()
                .Substring(messageSplit[2].Length).TrimStart()
                .Substring(":Your host is ".Length).Split(',')[0];
            var version = messageSplit.Last();

            return new(sender, target, serverName, version);
        }
    }
}