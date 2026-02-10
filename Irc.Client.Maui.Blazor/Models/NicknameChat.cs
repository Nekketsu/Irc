namespace Irc.Client.Maui.Blazor.Models;

public class NicknameChat : Chat
{
    public Nickname Nickname { get; }

    public NicknameChat(Pages.Index index, Nickname nickname) : base(index)
    {
        Nickname = nickname;

        Id = $"Nickname_{(string)nickname}";
        Name = (string)nickname;

        index.IrcClient.LocalUser.MessageReceived += LocalUser_MessageReceived;
    }

    private async void LocalUser_MessageReceived(object sender, Events.MessageEventArgs e)
    {
        if (e.From == Nickname)
        {
            await Speak(e.From, e.Message);
        }
    }
}
