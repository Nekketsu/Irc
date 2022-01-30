using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Irc.Server.Service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IrcServer ircServer;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            ircServer = new IrcServer(new ConsoleLogger());
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await ircServer.RunAsync(stoppingToken);
        }
    }
}
