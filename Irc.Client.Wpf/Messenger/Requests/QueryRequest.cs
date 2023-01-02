namespace Irc.Client.Wpf.Messenger.Requests
{
    public class QueryRequest
    {
        public string Nickname { get; }

        public QueryRequest(string nickname)
        {
            Nickname = nickname;
        }
    }
}
