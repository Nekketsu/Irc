using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Irc.Client.Wpf.Messages
{
    public class QueryRequestMessage
    {
        public string Nickname { get; }

        public QueryRequestMessage(string nickname)
        {
            Nickname = nickname;
        }
    }
}
