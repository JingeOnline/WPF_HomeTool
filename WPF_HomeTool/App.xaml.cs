
using Microsoft.AspNetCore.Hosting;
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
        //添加Web Api服务，想要添加必须在project中添加 <FrameworkReference Include="Microsoft.AspNetCore.App" />
        private static readonly IHost _webHost = Host.CreateDefaultBuilder().ConfigureWebHostDefaults(
            webBuilder =>
            {
                webBuilder.UseStartup<WebServerStartup>();
                webBuilder.UseUrls("http://*:5010");
            }).Build();
        //如果需要指定web api的配置文件，则使用下面的代码
        //.ConfigureAppConfiguration((hostingContext, config) => {
        //    config.Sources.Clear();//清除appsettings.josn中的配置源
        //    config.AddJsonFile("JingeConfig.json");
        //}).Build();

        private static readonly IHost _host = Host.CreateDefaultBuilder()
        .ConfigureServices((context, services) =>
        {
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
            _webHost.Start();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            // Stop the host and dispose
            _host.StopAsync().GetAwaiter().GetResult();
            _host.Dispose();
            _webHost.StopAsync().GetAwaiter().GetResult();
            _webHost.Dispose();
        }

    }

}
