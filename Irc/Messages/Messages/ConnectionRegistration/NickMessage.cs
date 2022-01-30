namespace Irc.Messages.Messages
{
    public class NickMessage : Message
    {
        public string Nickname { get; private set; }

        public NickMessage(string nickname)
        {
            Nickname = nickname;
        }

        public override string ToString()
        {
            return $"{Command} {Nickname}";
        }
    }
}