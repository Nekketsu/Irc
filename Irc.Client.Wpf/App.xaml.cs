using CommunityToolkit.Mvvm.Messaging;
using Irc.Client.Wpf.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace Irc.Client.Wpf
{
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

        private IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
            services.AddSingleton<IrcViewModel>();

            return services.BuildServiceProvider();
        }
    }
}
