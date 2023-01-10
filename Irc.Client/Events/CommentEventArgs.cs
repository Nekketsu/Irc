namespace Irc.Client.Events
{
    public class CommentEventArgs : EventArgs
    {
        public string Comment { get; }

        public CommentEventArgs(string comment)
        {
            Comment = comment;
        }
    }
}
