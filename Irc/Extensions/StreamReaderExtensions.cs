using System;
using System.IO;
using System.Threading.Tasks;
using Irc.Messages;

namespace Irc.Extensions
{
    public static class StreamReaderExtensions
    {
        public static async Task<Message> ReadMessageAsync(this StreamReader streamReader)
        {
            var text = await streamReader.ReadLineAsync();
            var message = Message.Parse(text);

            return message;
        }
    }
}