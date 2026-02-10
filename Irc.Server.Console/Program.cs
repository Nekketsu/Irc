namespace Irc.Server.Console;

class Program
{
    static async Task Main(string[] args)
    {
        var stoppingTokenSource = new CancellationTokenSource();
        System.Console.CancelKeyPress += (sender, args) => stoppingTokenSource.Cancel();

        var ircServer = new IrcServer(new ConsoleLogger());
        await ircServer.RunAsync(stoppingTokenSource.Token);

        Environment.Exit(0);
    }
}
