using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_HomeTool.Models;

namespace WPF_HomeTool.Helpers
{
    public class SecretHelper
    {
        public static string? OneDriveSecretFilePath;
        private static readonly ILogger<SecretHelper> _logger = LoggerFactory.Create(builder => builder.AddNLog()).CreateLogger<SecretHelper>();

        /// <summary>
        /// 获取OneDrive文件夹中储存的密钥，防止密钥泄露到GitHub上。
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetPropertyValueFromSecretJson(string appName, string key)
        {
            try
            {
                if (OneDriveSecretFilePath is null)
                {
                    string? oneDrivePath = Environment.GetEnvironmentVariable("OneDriveConsumer");
                    string secretFilePath = ConfigHelper.ReadKeyValue("SecretFilePath");
                    OneDriveSecretFilePath = Path.Combine(oneDrivePath, secretFilePath);
                }


                IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile(OneDriveSecretFilePath);
                IConfiguration config = builder.Build();
                var section = config.GetSection(appName).GetSection(key);
                return section.Value;
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex,$"获取OneDrive中Secret文件的数据时发生异常, appName={appName}, key={key}");
                throw;
            }
        }

        /// <summary>
        /// 从Secret文件中读取Office365 Graph的登录密钥
        /// </summary>
        /// <returns></returns>
        public static Office365SecretModel GetOffice365SecretModelFromSecretFile()
        {
            string appName = ConfigHelper.ReadKeyValue("Office365AccountSection");
            Office365SecretModel model = new Office365SecretModel();
            model.SecretValue = GetPropertyValueFromSecretJson(appName, "SecretValue");
            model.ClientId = GetPropertyValueFromSecretJson(appName, "ClientId");
            model.TenantId = GetPropertyValueFromSecretJson(appName, "TenantId");
            model.Scopes = GetPropertyValueFromSecretJson(appName, "Scopes");
            model.UserName = GetPropertyValueFromSecretJson(appName, "Username");
            return model;
        }
    }
}
