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

        public string ServerName { get; private set; }
        public Version Version { get; private set; }
        public DateTime CreatedDateTime { get; private set; }

        public List<IrcClient> IrcClients { get; set; }
        public Dictionary<string, Channel> Channels { get; set; }

        public IrcServer(int port = 6667)
        {
            ServerName = System.Environment.MachineName;
            Version = Assembly.GetExecutingAssembly().GetName().Version;

            this.port = port;
            
            IrcClients = new List<IrcClient>();
            IrcClient.IrcServer = this;
            Channels = new Dictionary<string, Channel>();
        }

        public async Task RunAsync()
        {
            var tcpListener = new TcpListener(IPAddress.Any, port);
            tcpListener.Start();
            CreatedDateTime = DateTime.Now;
            Console.WriteLine($"IRC Server started, {CreatedDateTime}");

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