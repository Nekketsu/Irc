using Irc.Bot.Console;
using Irc.Client;
using Irc.Messages;
using Irc.Messages.Messages;
using System;
using System.Diagnostics;

var apiKey = await File.ReadAllTextAsync("ApiKey.user");
var openAiService = new OpenAiService(apiKey);

var nickname = "Nekke-Bot";
var host = "irc.irc-hispano.org";

var ircClient = new IrcClient(nickname, host);

ircClient.MessageSent += IrcClient_MessageSent;
ircClient.MessageReceived += IrcClient_MessageReceived;

var cancellationTokenSource = new CancellationTokenSource();
await ircClient.RunAsync(cancellationTokenSource.Token);

var joinMessage = new JoinMessage("#Tgn");
await ircClient.SendMessageAsync(joinMessage);

while (true)
{
    var line = Console.ReadLine();
    var message = Message.Parse(line);
    if (message is not null)
    {
        await ircClient.SendMessageAsync(message);
    }
    else
    {
        Console.WriteLine($"Mensaje incorrecto: {line}");
    }
}


void IrcClient_MessageSent(object? sender, Message message)
{
    Debug.WriteLine($">> {message}");
}

async void IrcClient_MessageReceived(object? sender, Message message)
{
    Debug.WriteLine($"<< {message}");

    if (message is PrivMsgMessage privMsgMessage)
    {
        if (privMsgMessage.Target == nickname)
        {
            var now = TimeOnly.FromDateTime(DateTime.Now);
            var from = privMsgMessage.From.Split('!')[0];
            Console.WriteLine($"[{now}] <{from}> {privMsgMessage.Text}");

            var response = await openAiService.PostCompletionsAsync(privMsgMessage.Text);

            now = TimeOnly.FromDateTime(DateTime.Now);
            Console.WriteLine($"[{now}] <{nickname}> {response}");

            if (response is not null)
            {
                foreach (var responseSplit in response.Split('\n'))
                {
                    var responseMessage = new PrivMsgMessage(from, responseSplit);
                    await ircClient.SendMessageAsync(responseMessage);

                    await Task.Delay(TimeSpan.FromSeconds(0.5));
                }
            }
        }
    }
}