using Irc.Messages;
using Messages.Replies.ErrorReplies;

namespace Messages.Replies.CommandResponses
{
    [Command(RPL_WELCOME)]
    public class WelcomeReply : Reply
    {
        const string RPL_WELCOME = "001";

        const string DefaultMessage = "Welcome to the {0} Network, {1}";

        public string Message { get; }
        public string NetworkName { get; }
        public string Nickname { get; }

        public WelcomeReply(string sender, string target, string message) : base(sender, target, RPL_WELCOME)
        {
            Message = message;
            NetworkName = message.Substring("Welcome to the ".Length).Split()[0];
            Nickname = message.Split().Last();
        }

        public WelcomeReply(string sender, string target, string networkName, string nickname) : base(sender, target, RPL_WELCOME)
        {
            NetworkName = networkName;
            Nickname = nickname;
            Message = string.Format(DefaultMessage, networkName, nickname);
        }

        public override string InnerToString()
        {
            return $":{Message}";
        }

        public new static WelcomeReply Parse(string message)
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