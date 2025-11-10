using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPF_HomeTool.Models;

namespace WPF_HomeTool.Helpers
{
    public class ConfigHelper
    {
        public static Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        public static string GetConigFilePath()
        {
            return cfa.FilePath;
        }
        public static void CreatKeyValue(string key, string value)
        {
            cfa.AppSettings.Settings.Add(key, value);
            cfa.Save();
        }

        public static void SaveKeyValue(string key, string value)
        {
            cfa.AppSettings.Settings[key].Value = value;
            cfa.Save();
        }

        public static string? ReadKeyValue(string key)
        {
            return cfa.AppSettings.Settings[key]?.Value;
        }

        public static List<string>? ReadKeyValueIntoList(string key)
        {
            string? value = cfa.AppSettings.Settings[key]?.Value;
            return value?.Split(',', StringSplitOptions.TrimEntries)?.ToList();
        }

        public static List<KeyValueModel> GetAllKeyValuePairs()
        {
            List<KeyValueModel> keyValueList = new List<KeyValueModel>();
            foreach(var key in cfa.AppSettings.Settings.AllKeys)
            {
                keyValueList.Add(new KeyValueModel(key!, cfa.AppSettings.Settings[key]?.Value));
            }
            return keyValueList;
        }

        public static void SaveAllKeyValuePairs(IEnumerable<KeyValueModel> keyValueModels)
        {
            foreach (KeyValueModel keyValueModel in keyValueModels)
            {
                string valueInConfig=ReadKeyValue(keyValueModel.Key!)!;
                if(valueInConfig!=keyValueModel.Value)
                {
                    SaveKeyValue(keyValueModel.Key,keyValueModel.Value);
                }
            }
        }
    }
}