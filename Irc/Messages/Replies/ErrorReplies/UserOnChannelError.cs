using Irc.Messages;

namespace Messages.Replies.ErrorReplies
{
    public class UserOnChannelError : Reply
    {
        public const string DefaultMessage = "is already on channel";
        
        const string ERR_USERONCHANNEL = "443";
        public string Nickname { get; set; }
        public string ChannelName { get; set; }
        public string Message { get; private set; }

        public UserOnChannelError(string target, string nickname, string channelName, string message) : base(target, ERR_USERONCHANNEL)
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