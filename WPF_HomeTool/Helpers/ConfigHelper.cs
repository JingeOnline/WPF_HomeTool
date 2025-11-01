using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_HomeTool.Helpers
{
    public class ConfigHelper
    {
        public static void CreatKeyValue(string key, string value)
        {
            Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            cfa.AppSettings.Settings.Add(key, value);
            cfa.Save();
        }

        public static void SaveKeyValue(string key, string value)
        {
            Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            cfa.AppSettings.Settings[key].Value = value;
            cfa.Save();
        }

        public static string? ReadKeyValue(string key)
        {
            Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            return cfa.AppSettings.Settings[key]?.Value;
        }
    }
}
