using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Irc.Client.Wpf.ViewModels;

public partial class ConnectionDialogViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConnectCommand))]
    private string host;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConnectCommand))]
    private string nickname;

    [ObservableProperty]
    private int port = 6667;

    public bool ShouldConnect { get; private set; }

    public ConnectionDialogViewModel()
    {
    }

    public ConnectionDialogViewModel(string host, string nickname)
    {
        Host = host;
        Nickname = nickname;
    }

    [RelayCommand(CanExecute = nameof(CanConnect))]
    private void Connect()
    {
        ShouldConnect = true;
    }

    private bool CanConnect()
    {
        return !string.IsNullOrWhiteSpace(Host) && !string.IsNullOrWhiteSpace(Nickname);
    }
}
