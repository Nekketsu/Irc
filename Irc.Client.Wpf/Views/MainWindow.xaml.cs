using CommunityToolkit.Mvvm.Messaging;
using Irc.Client.Wpf.Messenger.Requests;
using Irc.Client.Wpf.ViewModels;
using System.Windows;

namespace Irc.Client.Wpf.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ScriptManagerDialog scriptManagerDialog;
        private ScriptManagerViewModel scriptManagerViewModel;
        private MainWindowViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();
        }

        public void Initialize(MainWindowViewModel mainWindowViewModel)
        {
            viewModel = mainWindowViewModel;
            DataContext = viewModel;

            var messenger = (IMessenger)((App)Application.Current).Services.GetService(typeof(IMessenger));

            // Register for dialog requests
            messenger.Register<ShowConnectionDialogRequest>(this, (r, m) => ShowConnectionDialog());
            messenger.Register<OpenScriptManagerDialogRequest>(this, (r, m) => OpenScriptManager());
            messenger.Register<ShowAboutDialogRequest>(this, (r, m) => ShowAboutDialog());
            messenger.Register<ExitApplicationRequest>(this, (r, m) => Application.Current.Shutdown());
        }

        private void ShowConnectionDialog()
        {
            var connectionDialogViewModel = new ConnectionDialogViewModel(viewModel.IrcViewModel.Host, viewModel.IrcViewModel.Nickname);
            var dialog = new ConnectionDialog
            {
                Owner = this,
                DataContext = connectionDialogViewModel
            };

            if (dialog.ShowDialog() == true)
            {
                viewModel.IrcViewModel.Host = connectionDialogViewModel.Host;
                viewModel.IrcViewModel.Nickname = connectionDialogViewModel.Nickname;

                if (connectionDialogViewModel.ShouldConnect && viewModel.IrcViewModel.ConnectCommand.CanExecute(null))
                {
                    viewModel.IrcViewModel.ConnectCommand.Execute(null);
                }
            }
        }

        private void OpenScriptManager()
        {
            if (scriptManagerDialog is not null && scriptManagerDialog.IsLoaded)
            {
                scriptManagerDialog.Activate();
                return;
            }

            if (scriptManagerViewModel is null)
            {
                scriptManagerViewModel = new ScriptManagerViewModel(viewModel.ScriptManager);
            }

            scriptManagerDialog = new ScriptManagerDialog
            {
                DataContext = scriptManagerViewModel,
                Owner = this
            };

            scriptManagerDialog.Closed += (s, args) => scriptManagerDialog = null;
            scriptManagerDialog.Show();
        }

        private void ShowAboutDialog()
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
    }
}
