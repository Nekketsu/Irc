namespace Irc.Messages
{
    public abstract class Reply : Message
    {
        public static string Sender { get; private set; }
        public string Target { get; private set; }
        public string ReplyCode { get; private set; }

        public Reply(string sender, string target, string replyCode)
        {
            Sender = sender;
            Target = target;
            ReplyCode = replyCode;
        }

        public override string ToString()
        {
            var text = $":{Sender} {ReplyCode} {Target}";
            var innerToString = InnerToString();

            if (innerToString != null)
            {
                text = $"{text} {innerToString}";
            }

            return text;
        }

        public abstract string InnerToString();
    }
}