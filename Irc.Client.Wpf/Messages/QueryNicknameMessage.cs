using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Irc.Client.Wpf.Messages
{
    public class QueryNicknameMessage
    {
        public string Nickname { get; }

        public QueryNicknameMessage(string nickname)
        {
            Nickname = nickname;
        }
    }
}
