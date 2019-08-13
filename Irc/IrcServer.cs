using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;

namespace Irc
{
    public class IrcServer
    {
        int port;
        public List<IrcClient> IrcClients { get; set; }

        public string HostName { get; private set; }
        public Version Version { get; private set; }
        public DateTime CreatedDateTime { get; private set; }

        public IrcServer(int port = 6667)
        {
            HostName = System.Environment.MachineName;
            Version = Assembly.GetExecutingAssembly().GetName().Version;

            this.port = port;
            
            IrcClients = new List<IrcClient>();
            IrcClient.IrcServer = this;
        }

        public async Task RunAsync()
        {
            Console.WriteLine("IRC Server started");

            var tcpListener = new TcpListener(IPAddress.Any, port);
            tcpListener.Start();

            while (true)
            {
                var client = await tcpListener.AcceptTcpClientAsync();
                Console.WriteLine($"Se ha conectado cliente {client.Client.LocalEndPoint}, {client.Client.RemoteEndPoint}");

                var ircClient = new IrcClient(client);
                IrcClients.Add(ircClient);
                ircClient.RunAsync();
            }
        }
    }
}