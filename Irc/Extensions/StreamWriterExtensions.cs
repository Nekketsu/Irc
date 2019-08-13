using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Irc.Messages;

namespace Irc.Extensions
{
    public static class StreamWriterExtensions
    {
        public static async Task WriteMessageAsync(this StreamWriter streamWriter, IMessage message)
        {
            await streamWriter.WriteLineAsync(message.ToString());
            Console.WriteLine($"Server: {message}");
        }

        public static async Task WriteMessageAsync(this StreamWriter streamWriter, IMessage message, CancellationToken cancellationToken)
        {
            await streamWriter.WriteLineAsync(message.ToString().AsMemory(), cancellationToken);
            Console.WriteLine($"Server: {message}");
        }
    }
}