namespace Irc.Messages.Messages
{
    public class NoticeMessage : Message
    {
        public string MsgTarget { get; set; }
        public string Text { get; set; }

        public NoticeMessage(string msgTarget, string text)
        {
            MsgTarget = msgTarget;
            Text = text;
        }

        public override string ToString()
        {
            return $"{Command} {MsgTarget} :{Text}";
        }
    }
}