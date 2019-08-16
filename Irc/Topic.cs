using System;

namespace Irc
{
    public class Topic
    {
        public string TopicMessage { get; private set; }
        public string NickName { get; set; }
        public DateTime SetAt { get; private set; }

        public Topic(string topicMessage, string nickName)
        {
            TopicMessage = TopicMessage;
            NickName = nickName;
            SetAt = DateTime.Now;
        }
    }
}