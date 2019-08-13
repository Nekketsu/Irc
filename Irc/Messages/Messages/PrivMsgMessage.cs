using System.Linq;
using System.Threading.Tasks;
using Irc.Extensions;
using Messages.Replies.ErrorReplies;

namespace Irc.Messages.Messages
{
    public class PrivMsgMessage : Message
    {
        public string From { get; set; }
        public string Target { get; set; }
        public string Text { get; set; }

        public PrivMsgMessage(string target, string text)
        {
            Target = target;
            Text = text.StartsWith(":") ? text.Substring(1) : text;
        }

        public PrivMsgMessage(string from, string target, string text) : this(target, text)
        {
            From = from;
        }

        public override string ToString()
        {
            return (From == null)
                ? $"{Command} {Target} :{Text}"
                : $"{From} {Command} {Target} :{Text}";
        }

        public override async Task<bool> ManageMessageAsync(IrcClient ircClient)
        {
            var targetClient = IrcClient.IrcServer.IrcClients.SingleOrDefault(client => client.Profile.NickName == Target);
            if (targetClient != null)
            {
                // var from = $"{ircClient.Profile.NickName}!{ircClient.Profile.User.UserName}@{ircClient.Address}";
                var from = ircClient.Profile.NickName;
                var privMsgMessage = new PrivMsgMessage(from, Target, Text);
                await targetClient.StreamWriter.WriteMessageAsync(privMsgMessage);
            }
            else
            {
                await ircClient.StreamWriter.WriteMessageAsync(new NoRecipientError($":No recipient given ({this})"));
            }

            return true;
        }

        public new static PrivMsgMessage Parse(string message)
        {
            var messageSplit = message.Split();

            var target = messageSplit[1];

            var text = message.Substring(message.IndexOf(messageSplit[0]) + messageSplit[0].Length).TrimStart();
            text = message.Substring(message.IndexOf(messageSplit[1]) + messageSplit[1].Length).TrimStart();

            return new PrivMsgMessage(target, text);
        }
    }
}