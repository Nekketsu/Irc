using Irc.Client.Events;

namespace Irc.Client
{
    public class Channel
    {
        private static char[] channelPrefixes = { '#', '&' };

        public string Name { get; set; }
        public UserCollection Users { get; set; }
        public string Topic { get; set; }

        public event EventHandler<MessageEventArgs> MessageReceived;
        public event EventHandler<MessageEventArgs> NoticeReceived;
        public event EventHandler<UserEventArgs> UserJoined;
        public event EventHandler<UserEventArgs> UserLeft;

        public Channel()
        {
            Users = new();
        }

        public static bool IsChannel(string target) => channelPrefixes.Any(target.StartsWith);

        internal void OnMessageReceived(Nickname nickname, string text)
        {
            MessageReceived?.Invoke(this, new(nickname, text));
        }

        internal void OnNoticeReceived(Nickname nickname, string text)
        {
            NoticeReceived?.Invoke(this, new(nickname, text));
        }

        internal void OnUserJoined(Nickname nickname)
        {
            UserJoined?.Invoke(this, new(nickname));
        }

        internal void OnUserLeft(Nickname nickname)
        {
            UserLeft?.Invoke(this, new(nickname));
        }
    }
}
