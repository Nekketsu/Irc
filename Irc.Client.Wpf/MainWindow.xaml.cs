using CommunityToolkit.Mvvm.Messaging;
using Irc.Client.Wpf.Messenger.Requests;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace Irc.Client.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var messenger = App.Current.Services.GetService<IMessenger>();

            messenger.Register<TitleRequest>(this, (r, m) =>
            {
                Title = m.Title;
            });
        }
    }
}
