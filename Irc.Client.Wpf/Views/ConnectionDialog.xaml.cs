using Irc.Client.Wpf.ViewModels;
using System.Windows;

namespace Irc.Client.Wpf.Views;

public partial class ConnectionDialog : Window
{
    public ConnectionDialogViewModel ViewModel => DataContext as ConnectionDialogViewModel;

    public ConnectionDialog()
    {
        InitializeComponent();
        Loaded += ConnectionDialog_Loaded;
    }

    private void ConnectionDialog_Loaded(object sender, RoutedEventArgs e)
    {
        HostTextBox.Focus();
    }

    private void Connect_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }
}
