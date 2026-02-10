using Irc.Client.Wpf.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;

namespace Irc.Client.Wpf.Views;

/// <summary>
/// Interaction logic for IrcView.xaml
/// </summary>
public partial class IrcView : UserControl
{
    public IrcView()
    {
        InitializeComponent();

        DataContext = App.Current.Services.GetService<IrcViewModel>();
    }
}
