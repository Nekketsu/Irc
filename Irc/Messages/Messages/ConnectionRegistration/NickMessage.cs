namespace Irc.Messages.Messages
{
    [Command("NICK")]
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