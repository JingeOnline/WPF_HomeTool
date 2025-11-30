using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_HomeTool.Controls;
using WPF_HomeTool.Helpers;

namespace WPF_HomeTool.Services
{
    internal class SyncConfigFileService : BackgroundService
    {
        private readonly ILogger _logger;

        public SyncConfigFileService(ILogger<SyncConfigFileService> logger)
        {
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await Task.Run(() =>
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
                                //将配置文件复制到OneDrive指定文件
                                string configFilePath = ConfigHelper.GetConigFilePath();
                                string destFilePath = Path.Combine(targetFolderPath, "App.config");
                                File.Copy(configFilePath, destFilePath, true);

                                //同步Saves文件夹
                                string saveFolder = ConfigHelper.ReadKeyValue("SavesFolder");
                                string localDownloadAlbumsPath = ConfigHelper.ReadKeyValue("ImageFapDownloadAlbumUrlPath");
                                string localUndownloadFilesPath = ConfigHelper.ReadKeyValue("ImageFapUndownloadFilePath");
                                string oneDriveDownloadAlbumsPath = Path.Combine(targetFolderPath, "ImageFap_DownloadAlbums.txt");
                                string oneDriveUndownloadFilesPath = Path.Combine(targetFolderPath, "ImageFap_UndownloadFiles.csv");
                                //如果本地Saves文件不存在（第一次启动），则拷贝OneDrive中的文件到本地
                                if (!Directory.Exists(saveFolder))
                                {
                                    Controls.MessageBoxResult result=Controls.MessageBoxResult.None;
                                    Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        result = ModernMessageBox.Show("本地不存在Saves文件，是否导入OneDrive中的Saves文件到当前程序中？", "新应用提示"
                                            , Controls.MessageBoxButton.YesNo);
                                    });
                                    if (result == Controls.MessageBoxResult.Yes)
                                    {
                                        Directory.CreateDirectory(saveFolder);
                                        if (File.Exists(oneDriveDownloadAlbumsPath))
                                        {
                                            File.Copy(oneDriveDownloadAlbumsPath, localDownloadAlbumsPath, true);
                                        }
                                        if (File.Exists(oneDriveUndownloadFilesPath))
                                        {
                                            File.Copy(oneDriveUndownloadFilesPath, localUndownloadFilesPath, true);
                                        }
                                    }
                                }
                                //如果本地Save文件已经存在，则复制到OneDrive中
                                else
                                {
                                    if (File.Exists(localDownloadAlbumsPath))
                                    {
                                        File.Copy(localDownloadAlbumsPath, oneDriveDownloadAlbumsPath, true);
                                    }
                                    if (File.Exists(localUndownloadFilesPath))
                                    {
                                        File.Copy(localUndownloadFilesPath, oneDriveUndownloadFilesPath, true);
                                    }
                                }




                                ////获取当前应用程序exe文件所在的目录
                                //string exePath = System.Environment.ProcessPath!;
                                ////获取文件夹路径
                                //string? exeDirectory = Path.GetDirectoryName(exePath);
                                //Debug.WriteLine($"Current exe path: {exePath}");
                                ////组合配置文件的完整路径
                                //string sourceFilePath = Path.Combine(exeDirectory, "App.config");

                            }
                        }
                    }
                }, stoppingToken).ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                //Debug.WriteLine("同步Config被取消");
                _logger.LogInformation("向OneDrive中同步Config文件被取消");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "向OneDrive中同步Config文件出现异常");
                //Debug.WriteLine($"同步Config发生异常: {ex.Message}");
            }
        }
    }
}
