using Irc.Messages;

namespace Messages.Replies.ErrorReplies
{
    public class UserNotInChannelError : Reply
    {
        public const string DefaultMessage = "They aren't on that channel";
        
        const string ERR_USERNOTINCHANNEL = "441";
        public string Nickname { get; set; }
        public string ChannelName { get; set; }
        public string Message { get; private set; }

        public UserNotInChannelError(string sender, string target, string nickname, string channelName, string message) : base(sender, target, ERR_USERNOTINCHANNEL)
        {
            Nickname = nickname;
            ChannelName = channelName;
            Message = message;
        }

        public override string InnerToString()
        {
            return $"{Nickname} {ChannelName} :{Message}";
        }
    }
}