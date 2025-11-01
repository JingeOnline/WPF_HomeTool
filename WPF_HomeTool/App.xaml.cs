using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows;
using WPF_HomeTool.Navigation;
using WPF_HomeTool.Services;
using WPF_HomeTool.ViewModels;
using WPF_HomeTool.Views;

namespace WPF_HomeTool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly IHost _host = Host.CreateDefaultBuilder()
        .ConfigureServices((context, services) => {
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<WebServerPage>();
            services.AddSingleton<WebServerPageViewModel>();
            services.AddSingleton<HomePage>();
            services.AddSingleton<HomePageViewModel>();
            services.AddSingleton<SettingsPage>();
            services.AddSingleton<SettingsPageViewModel>();

            services.AddHostedService<SyncConfigFileService>();
        }).Build();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _host.Start();
            //指定启动窗口
            this.MainWindow = _host.Services.GetRequiredService<MainWindow>();
            this.MainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            // Stop the host and dispose
            _host.StopAsync().GetAwaiter().GetResult();
            _host.Dispose();
        }

    }

}
