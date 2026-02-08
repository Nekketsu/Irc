using CommunityToolkit.Mvvm.Messaging;
using Irc.Client.Scripting;
using Irc.Client.Wpf.Messenger.Requests;
using Irc.Client.Wpf.ViewModels;
using Irc.Client.Wpf.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace Irc.Client.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ScriptManagerWindow scriptManagerWindow;
        private ScriptManagerViewModel scriptManagerViewModel;

        public MainWindow()
        {
            InitializeComponent();

            var messenger = App.Current.Services.GetService<IMessenger>();

            messenger.Register<TitleRequest>(this, (r, m) =>
            {
                Title = m.Title;
            });
        }

        private ScriptManager GetOrCreateScriptManager()
        {
            // Always use the ScriptManager from IrcViewModel to ensure single source of truth
            var ircViewModel = App.Current.Services.GetService<IrcViewModel>();
            return ircViewModel?.ScriptManager;
        }

        private void ScriptManager_Click(object sender, RoutedEventArgs e)
        {
            // Si ya existe una ventana, traerla al frente
            if (scriptManagerWindow is not null && scriptManagerWindow.IsLoaded)
            {
                scriptManagerWindow.Activate();
                return;
            }

            // Obtener o crear ScriptManager y ViewModel (reutilizar si ya existe)
            var manager = GetOrCreateScriptManager();
            if (scriptManagerViewModel is null)
            {
                scriptManagerViewModel = new ScriptManagerViewModel(manager);
            }

            scriptManagerWindow = new ScriptManagerWindow
            {
                DataContext = scriptManagerViewModel,
                Owner = this
            };

            scriptManagerWindow.Closed += (s, args) => scriptManagerWindow = null;
            scriptManagerWindow.Show();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "IRC Client with Scripting Support\n\n" +
                "Developed with .NET 10.0\n" +
                "Scripting powered by Roslyn\n\n" +
                "Use Tools > Script Manager to automate tasks with C# scripts.",
                "About IRC Client",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
