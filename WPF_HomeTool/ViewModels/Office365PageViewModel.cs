using Azure.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Protection.PlayReady;
using WPF_HomeTool.Controls;
using WPF_HomeTool.Helpers;
using WPF_HomeTool.Models;

namespace WPF_HomeTool.ViewModels
{
    public partial class Office365PageViewModel : ObservableObject
    {
        private readonly ILogger<Office365PageViewModel> _logger;
        private GraphServiceClient graphClient;
        [ObservableProperty]
        private string _ConsoleOutputText = string.Empty;

        public Office365PageViewModel(ILogger<Office365PageViewModel> logger)
        {
            _logger = logger;
            LoginToOfficeAccountAsync();
        }

        private async Task LoginToOfficeAccountAsync()
        {
            Office365SecretModel secretModel;
            try
            {
                secretModel = SecretHelper.GetOffice365SecretModelFromSecretFile();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "从 OneDrive Secret 文件中获取 Office 365 Graph 密钥出现异常");
                ModernMessageBox.Show(ex.Message + Environment.NewLine + ex.InnerException?.Message, "从 OneDrive Secret 文件中获取 Office 365 Graph 密钥出现异常");
                return;
            }

            try
            {
                ClientSecretCredential credential = new ClientSecretCredential(secretModel.TenantId, secretModel.ClientId, secretModel.SecretValue);
                graphClient = new GraphServiceClient(credential);
                await GetDriveFromOneDriveAsync(secretModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "登录OneDrive发生异常");
                ModernMessageBox.Show(ex.Message + Environment.NewLine + ex.InnerException?.Message, "登录OneDrive发生异常");
                return;
            }
        }


        private async Task GetDriveFromOneDriveAsync(Office365SecretModel secretModel)
        {
            ConsoleOutputText = "请留意密钥到期时间2027年12月6号，请及时去Azure更新应用程序的密钥。" + Environment.NewLine;
            ConsoleOutputText = "正在登录OneDrive..." + Environment.NewLine;
            //这一步获取网盘的ID，然后后面通过ID来获取根目录下的文件。即使只有一个网盘，也需要这样才行。
            Drive drive = await graphClient.Users[secretModel.UserName].Drive.GetAsync();
            ConsoleOutputText += $"登录成功！获取到OneDrive网盘ID: {drive.Id}" + Environment.NewLine;
            if (drive?.Quota != null)
            {
                long used = drive.Quota.Used ?? 0;
                long total = drive.Quota.Total ?? 0;
                long remaining = drive.Quota.Remaining ?? 0;
                string state = drive.Quota.State; // normal, nearing, critical, exceeded

                ConsoleOutputText += $"Drive: {drive.Name}"+ Environment.NewLine;
                ConsoleOutputText += $"已使用: {FileHelper.ConvertBytesToHumanReadable(used)}" + Environment.NewLine;
                ConsoleOutputText += $"总容量: {FileHelper.ConvertBytesToHumanReadable(total)}" + Environment.NewLine;
                ConsoleOutputText += $"剩余: {FileHelper.ConvertBytesToHumanReadable(remaining)}" + Environment.NewLine;
                ConsoleOutputText += $"使用率: {GetUsagePercentage(used, total):F2}%" + Environment.NewLine;
                ConsoleOutputText += $"状态: {state}" + Environment.NewLine;
            }


            //var users = await graphClient.Users.GetAsync();
            //foreach (var user in users.Value)
            //{
            //    ConsoleOutputText += ($"ID: {user.Id}, Name: {user.DisplayName}, Email: {user.UserPrincipalName}") + Environment.NewLine;
            //}

        }
        // 计算使用百分比
        private double GetUsagePercentage(long used, long total)
        {
            if (total == 0) return 0;
            return (double)used / total * 100;
        }
    }
}
