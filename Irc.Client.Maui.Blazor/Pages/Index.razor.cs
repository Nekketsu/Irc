using Irc.Client.Maui.Blazor.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Diagnostics;

namespace Irc.Client.Maui.Blazor.Pages;

public partial class Index
{
    [Inject]
    public IJSRuntime JSRuntime { get; set; }

    public IrcClient IrcClient { get; set; }
    private Task ircClientTask;
    CancellationTokenSource cancellationTokenSource;

    public string Host { get; set; } = "Nekketsu";
    public string Nickname { get; set; } = "irc.irc-hispano.org";

    public bool IsConnected { get; set; } = false;

    public Chat Status { get; set; }
    public List<Chat> Chats { get; set; }
    public Chat CurrentChat { get; set; }

    public string Message { get; set; }

    public void Connect()
    {
        cancellationTokenSource = new CancellationTokenSource();

        IrcClient = new(Host, Nickname);

        IrcClient.Connected += IrcClient_Connected;
        //IrcClient.MessageSent += IrcClient_MessageSent;
        //IrcClient.MessageReceived += IrcClient_MessageReceived;
        IrcClient.RawMessageSent += IrcClient_RawMessageSent;
        IrcClient.RawMessageReceived += IrcClient_RawMessageReceived;
        IrcClient.LocalUser.ChannelJoined += LocalUser_ChannelJoined;

        Status = new Chat(this, "Status");
        Chats = [Status];
        CurrentChat = Status;

        ircClientTask = IrcClient.RunAsync(cancellationTokenSource.Token);
    }

    public void OnStateHasChanged()
    {
        StateHasChanged();
    }

    private void LocalUser_ChannelJoined(object sender, Events.ChannelEventArgs e)
    {
        var chat = new ChannelChat(this, e.Channel);
        Chats.Add(chat);
        CurrentChat = chat;

        StateHasChanged();
    }

    private void IrcClient_Connected(object sender, EventArgs e)
    {
        IsConnected = true;

        StateHasChanged();
    }

    private void IrcClient_RawMessageReceived(object sender, string message)
    {
        Debug.WriteLine(message);
        Status.Log.Add(new ChatMessage(message));

        if (CurrentChat == Status)
        {
            StateHasChanged();
        }

        JSRuntime.InvokeVoidAsync("scrollToBottom", Status.Id);
    }

    private void IrcClient_RawMessageSent(object sender, string message)
    {
        Status.Log.Add(new ChatMessage(message));

        if (CurrentChat == Status)
        {
            StateHasChanged();
        }

        JSRuntime.InvokeVoidAsync("scrollToBottom", Status.Id);
    }

    private async Task Message_KeyDown(KeyboardEventArgs e)
    {
        if ((e.Code == "Enter") || (e.Code == "NumEnter"))
        {
            await SendMessage();
        }
    }

    private async Task SendMessage()
    {
        var message = Messages.Message.Parse(Message);
        if (message is not null)
        {
            await IrcClient.SendMessageAsync(message);
            Message = null;
        }
    }
}
