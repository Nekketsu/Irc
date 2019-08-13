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

            if (message != null)
            {
                Console.WriteLine($"Client: {text}");
            }
            else
            {
                var foregroundColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"Client: {text}");
                Console.ForegroundColor = foregroundColor;
            }

            return message;
        }
    }
}