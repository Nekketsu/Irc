using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    [Command(RPL_YOURHOST)]
    public class YourHostReply : Reply
    {
        const string RPL_YOURHOST = "002";

        const string DefaultMessage = "Your host is {0}, running version {1}";

        public string Message { get; }
        public string ServerName { get; }
        public string Version { get; }

        public YourHostReply(string sender, string target, string message) : base(sender, target, RPL_YOURHOST)
        {
            Message = message;
            ServerName = message.Substring("Your host is ".Length).Split(',')[0];
            Version = message.Split().Last();
        }

        public YourHostReply(string sender, string target, string serverName, string version) : base(sender, target, RPL_YOURHOST)
        {
            ServerName = serverName;
            Version = version;
            Message = string.Format(DefaultMessage, serverName, version);
        }

        public override string InnerToString()
        {
            return $":{Message}";
        }

        public new static YourHostReply Parse(string message)
        {
            var messageSplit = message.Split();

            var sender = messageSplit[0];
            var target = messageSplit[2];
            var text = message
                .Substring(messageSplit[0].Length).TrimStart()
                .Substring(messageSplit[1].Length).TrimStart()
                .Substring(messageSplit[2].Length).TrimStart()
                .Substring(":".Length);

            return new(sender, target, text);
        }
    }
}