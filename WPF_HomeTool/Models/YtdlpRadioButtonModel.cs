using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WPF_HomeTool.Models
{
    public partial class YtdlpRadioButtonModel : ObservableObject
    {
        [ObservableProperty]
        private string _name;
        [ObservableProperty]
        private bool _isChecked;
        public string Command { get; set; }

        public YtdlpRadioButtonModel(string name, string command, bool isChecked=false)
        {
            Name = name;
            IsChecked = isChecked;
            Command = command;
        }
    }

    public partial class YtdlpSubtitleLanguageRadioButtonModel : ObservableObject
    {
        [ObservableProperty]
        private string _name;
        [ObservableProperty]
        private bool _isChecked;
        public string Command { get; set; }

        public YtdlpSubtitleLanguageRadioButtonModel(string name, string command, bool isChecked = false)
        {
            Name = name;
            IsChecked = isChecked;
            Command = command;
        }
    }
}
