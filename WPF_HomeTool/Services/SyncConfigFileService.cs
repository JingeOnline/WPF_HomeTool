using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_HomeTool.Helpers;

namespace WPF_HomeTool.Services
{
    internal class SyncConfigFileService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await Task.Run(async () =>
                {
                    //必须传入CancellationToken才能在App.xaml.cs中停止时，触发取消任务。否则任务不能被终止。
                    //await Task.Delay(10000,stoppingToken);
                    //Debug.WriteLine("10 SECONDS stop");
                    string? OneDrivePath = Environment.GetEnvironmentVariable("OneDriveConsumer");
                    if (!string.IsNullOrEmpty(OneDrivePath))
                    {
                        string? subPath = ConfigHelper.ReadKeyValue("ConfigFileBackupPath");
                        if (!string.IsNullOrEmpty(subPath))
                        {
                            string targetFolderPath = Path.Combine(OneDrivePath, subPath);
                            if (Directory.Exists(targetFolderPath))
                            {
                                //获取当前应用程序exe文件所在的目录
                                string exePath = System.Environment.ProcessPath!;
                                //获取文件夹路径
                                string? exeDirectory = Path.GetDirectoryName(exePath);
                                Debug.WriteLine($"Current exe path: {exePath}");
                                //组合配置文件的完整路径
                                string sourceFilePath = Path.Combine(exeDirectory, "App.config");
                                //将配置文件复制到OneDrive指定文件
                                string destFilePath = Path.Combine(targetFolderPath, "App.config");
                                File.Copy(sourceFilePath, destFilePath, true);
                            }
                        }
                    }
                }, stoppingToken).ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                Debug.WriteLine("同步Config被取消");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"同步Config发生异常: {ex.Message}");
            }
        }
    }
}
