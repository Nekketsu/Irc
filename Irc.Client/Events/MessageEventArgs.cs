namespace Irc.Client.Events
{
    public class MessageEventArgs : EventArgs
    {
        public Nickname From { get; }
        public string Message { get; }

        public MessageEventArgs(Nickname from, string message)
        {
            From = from;
            Message = message;
        }
    }
}
