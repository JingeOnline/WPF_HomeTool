using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");
//创建一个Host和BackgroundService示例
var host = Host.CreateDefaultBuilder()
    .ConfigureServices((context, services) =>
    {
        services.AddHostedService<DemoBackgroundService>();
    })
    .Build();
host.Start();
await Task.Delay(5000);
host.StopAsync().GetAwaiter().GetResult();

internal class DemoBackgroundService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Your background task logic here
        while (!stoppingToken.IsCancellationRequested)
        {
            // Simulate work
            try
            {
                await Task.Delay(1000, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("Task cancelled.");
            }
            System.Console.WriteLine(DateTime.Now + "Background service is running...");
        }
        Console.WriteLine("CancellationToken cancelled.");
        return;
    }
}