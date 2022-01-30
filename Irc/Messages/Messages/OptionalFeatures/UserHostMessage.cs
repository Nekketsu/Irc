namespace Irc.Messages.Messages
{
    public class UserHostMessage : Message
    {
        public string Nickname { get; set; }

        public UserHostMessage(string nickname)
        {
            Nickname = nickname;
        }

        public override string ToString()
        {
            return $"{Command} {Nickname}";
        }
    }
}