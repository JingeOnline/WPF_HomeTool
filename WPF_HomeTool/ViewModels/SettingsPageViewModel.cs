using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_HomeTool.Helpers;
using WPF_HomeTool.Models;

namespace WPF_HomeTool.ViewModels
{
    public partial class SettingsPageViewModel : ObservableObject
    {
        //[ObservableProperty]
        //private string _pageTitle = "Settings 1234";

        //[ObservableProperty]
        //private string _pageDescription = "Settings 1234567890";

        [ObservableProperty]
        private ObservableCollection<KeyValueModel> keyValueCollection;

        public SettingsPageViewModel()
        {
            KeyValueCollection=new ObservableCollection<KeyValueModel>(ConfigHelper.GetAllKeyValuePairs());
        }

        [RelayCommand]
        private void SaveConfig()
        {
            ConfigHelper.SaveAllKeyValuePairs(keyValueCollection);
        }
        [RelayCommand]
        private void ReloadConfig()
        {
            KeyValueCollection.Clear();
            KeyValueCollection = new ObservableCollection<KeyValueModel>(ConfigHelper.GetAllKeyValuePairs());
        }
    }
}
