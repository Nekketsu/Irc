using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Irc.Client.Scripting;
using Irc.Client.Wpf.Messenger.Requests;
using Irc.Client.Wpf.Model;

namespace Irc.Client.Wpf.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly IMessenger messenger;
    private readonly IrcViewModel ircViewModel;

    [ObservableProperty]
    private string title = "IRC Client";

    public IrcViewModel IrcViewModel => ircViewModel;

    public ScriptManager ScriptManager => ircViewModel?.ScriptManager;

    public MainWindowViewModel(IMessenger messenger, IrcViewModel ircViewModel)
    {
        this.messenger = messenger;
        this.ircViewModel = ircViewModel;

        // Listen for title updates from legacy code
        messenger.Register<TitleRequest>(this, (r, m) =>
        {
            Title = m.Title;
        });

        // Listen for connection state changes to update title
        if (ircViewModel is not null)
        {
            ircViewModel.PropertyChanged += IrcViewModel_PropertyChange;
            UpdateTitle();
        }
    }

    private void IrcViewModel_PropertyChange(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IrcViewModel.State))
        {
            UpdateTitle();
        }
    }

    private void UpdateTitle()
    {
        if (ircViewModel.State == ConnectionState.Connecting)
        {
            Title = $"IRC Client - {ircViewModel.Nickname} @ {ircViewModel.Host}";
        }
        else
        {
            Title = $"IRC Client";
        }
    }

    [RelayCommand]
    private void ShowConnection()
    {
        messenger.Send(new ShowConnectionDialogRequest());
    }

    [RelayCommand]
    private void ShowScriptManager()
    {
        messenger.Send(new OpenScriptManagerDialogRequest());
    }

    [RelayCommand]
    private void ShowAbout()
    {
        messenger.Send(new ShowAboutDialogRequest());
    }

    [RelayCommand]
    private void Exit()
    {
        messenger.Send(new ExitApplicationRequest());
    }
}
