namespace Irc.Client.Wpf.Messages
{
    public class WhoisRequestMessage
    {
        public string Nickname { get; }

        public WhoisRequestMessage(string nickname)
        {
            Nickname = nickname;
        }
    }
}
