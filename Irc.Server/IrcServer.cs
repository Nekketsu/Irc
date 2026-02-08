using Irc.Domain.Extensions;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace Irc.Server
{
    public class IrcServer
    {
        public ILogger Logger { get; }
        private int port;

        public string ServerName { get; private set; }
        public Version Version { get; private set; }
        public DateTime CreatedDateTime { get; private set; }

        public List<IrcClient> IrcClients { get; set; }
        public Dictionary<string, Channel> Channels { get; set; }

        public IrcServer(ILogger logger, int port = Protocol.DefaultPort)
        {
            Logger = logger;

            ServerName = Environment.MachineName;
            Version = Assembly.GetExecutingAssembly().GetName().Version;

            this.port = port;

            IrcClients = new List<IrcClient>();
            Channels = new Dictionary<string, Channel>();
        }

        public async Task RunAsync(CancellationToken stoppingToken)
        {
            var tcpListener = new TcpListener(IPAddress.Any, port);
            try
            {
                tcpListener.Start();
                CreatedDateTime = DateTime.Now;
                Logger.LogDebug($"IRC Server started, {CreatedDateTime}");

                while (!stoppingToken.IsCancellationRequested)
                {
                    var client = await tcpListener.AcceptTcpClientAsync().WithCancellation(stoppingToken);

                    var ircClient = new IrcClient(this, client);
                    IrcClients.Add(ircClient);
                    ircClient.RunAsync(stoppingToken);
                }
            }
            catch (OperationCanceledException) { }
            finally
            {
                tcpListener.Stop();
            }

            Logger.LogDebug($"IRC Server stopped, {CreatedDateTime}");
        }
    }
}