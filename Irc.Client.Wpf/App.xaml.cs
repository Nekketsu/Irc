using CommunityToolkit.Mvvm.Messaging;
using Irc.Client.Wpf.ViewModels;
using Irc.Client.Wpf.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace Irc.Client.Wpf;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public IServiceProvider Services { get; }

    public new static App Current => (App)Application.Current;

    public App()
    {
        Services = ConfigureServices();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var mainWindow = new MainWindow();
        var mainWindowViewModel = Services.GetRequiredService<MainWindowViewModel>();
        mainWindow.Initialize(mainWindowViewModel);
        mainWindow.Show();

        MainWindow = mainWindow;
    }

    private IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
        services.AddSingleton<IrcViewModel>();
        services.AddSingleton<MainWindowViewModel>();

        return services.BuildServiceProvider();
    }
}
