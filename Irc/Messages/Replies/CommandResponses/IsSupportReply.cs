using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    [Command(RPL_ISUPPORT)]
    public class IsSupportReply : Reply
    {
        const string RPL_ISUPPORT = "005";

        public string[] Parameters { get; }

        public IsSupportReply(string sender, string target, string[] parameters) : base(sender, target, RPL_ISUPPORT)
        {
            Parameters = parameters;
        }

        public override string InnerToString()
        {
            return $"{string.Join(' ', Parameters)} :are supported by this server";
        }

        public new static IsSupportReply Parse(string message)
        {
            message = message.Substring(0, message.Length - " :are supported by this server".Length);

            var messageSplit = message.Split();

            var sender = messageSplit[0];
            var target = messageSplit[2];
            var parameters = messageSplit.Skip(3).ToArray();

            return new(sender, target, parameters);
        }
    }
}