using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_HomeTool.Models
{
    public partial class KeyValueModel:ObservableObject
    {
        [ObservableProperty]
        private string _key;
        [ObservableProperty]
        private string _value;

        public KeyValueModel(string key, string value)
        {
            Key=key;
            Value=value;
        }
    }
}
