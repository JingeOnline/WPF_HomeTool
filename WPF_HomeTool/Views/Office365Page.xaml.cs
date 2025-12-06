using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPF_HomeTool.ViewModels;

namespace WPF_HomeTool.Views
{
    public partial class Office365Page : Page
    {
        private readonly ILogger<Office365Page> _logger;
        public Office365PageViewModel VM { get; }
        public Office365Page(Office365PageViewModel vm, ILogger<Office365Page> logger)
        {
            VM = vm;
            _logger = logger;
            DataContext = this;
            InitializeComponent();
        }
    }
}
