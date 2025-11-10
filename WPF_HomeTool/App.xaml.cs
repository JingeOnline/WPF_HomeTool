
using Microsoft.AspNetCore.Hosting;
using System.IO;
using WPF_HomeTool.Services;
using WPF_HomeTool.ViewModels;
using WPF_HomeTool.Views;
using WPF_HomeTool.Controls;
using WPF_HomeTool.Helpers;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;


namespace WPF_HomeTool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();


        //添加Web Api服务，想要添加必须在project中添加 <FrameworkReference Include="Microsoft.AspNetCore.App" />
        private static readonly IHost _webHost = Host.CreateDefaultBuilder().ConfigureWebHostDefaults(
            webBuilder =>
            {
                webBuilder.UseStartup<WebServerStartup>();
                string port = ConfigHelper.ReadKeyValue("ApiPort")!;
                webBuilder.UseUrls("http://*:" + port);
            }).Build();
        //如果需要指定web api的配置文件，则使用下面的代码
        //.ConfigureAppConfiguration((hostingContext, config) => {
        //    config.Sources.Clear();//清除appsettings.josn中的配置源
        //    config.AddJsonFile("JingeConfig.json");
        //}).Build();

        private static readonly IHost _host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<WPF_HomeTool.Navigation.INavigationService, WPF_HomeTool.Navigation.NavigationService>();
                services.AddSingleton<MainWindow>();
                services.AddSingleton<MainWindowViewModel>();
                services.AddSingleton<WebServerPage>();
                services.AddSingleton<WebServerPageViewModel>();
                services.AddSingleton<HomePage>();
                services.AddSingleton<HomePageViewModel>();
                services.AddSingleton<SettingsPage>();
                services.AddSingleton<SettingsPageViewModel>();
                services.AddSingleton<FilesRenamePage>();
                services.AddSingleton<FilesRenamePageViewModel>();
                services.AddSingleton<WebViewScraperPage>();
                services.AddSingleton<WebViewScraperPageViewModel>();

                services.AddHostedService<SyncConfigFileService>();
                //添加Nlog服务
                //注意创建nlog.config文件后，其Build Action必须保留None，并选择始终复制。
                services.AddLogging(loggingBuilder => {
                    loggingBuilder.ClearProviders();
                    loggingBuilder.AddNLog();
                });
            }).Build();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _host.Start();
            //指定启动窗口
            this.MainWindow = _host.Services.GetRequiredService<MainWindow>();
            this.MainWindow.Show();
            //_webHost.Start();
            try
            {
                _webHost.Start();
            }
            catch (IOException ex)
            {
                ModernMessageBox.Show(ex.Message, "API端口已被占用", Controls.MessageBoxButton.OK, Controls.MessageBoxImage.Warning);
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            MyOnExitAsync().GetAwaiter().GetResult();

        }

        //为了避免退出的时候在UI线程发生死锁，所以在这里统一执行异步方法，最后配置ConfigureAwait(false)
        async Task MyOnExitAsync()
        {
            await Task.Run(async () =>
            {
                //await Task.Delay(5000);
                await _host.StopAsync();
                _host.Dispose();
                await _webHost.StopAsync();
                _webHost.Dispose();
            }).ConfigureAwait(false);
        }


        void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            logger.Error(e.Exception,"程序异常退出");
        }
    }

}
