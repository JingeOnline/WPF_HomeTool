using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_HomeTool.Models
{
    public partial class RadioButtonItemModel : ObservableObject
    {
        [ObservableProperty]
        private string _name;
        [ObservableProperty]
        private bool _isChecked;
        public string Command { get; set; }

        public RadioButtonItemModel(string name, string command)
        {
            Name = name;
            IsChecked = false;
            Command = command;
        }
    }
}
