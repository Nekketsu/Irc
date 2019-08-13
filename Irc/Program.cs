using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Irc
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var ircServer = new IrcServer();
            await ircServer.RunAsync();
        }
    }
}
