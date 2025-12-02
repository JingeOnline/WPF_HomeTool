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
    /// <summary>
    /// Interaction logic for ZipPage.xaml
    /// </summary>
    public partial class ZipPage : Page
    {
        private readonly ILogger<ZipPage> _logger;
        public ZipPageViewModel VM { get; }
        public ZipPage(ZipPageViewModel vm, ILogger<ZipPage> logger)
        {
            _logger = logger;
            VM = vm;
            DataContext = this;
            InitializeComponent();
        }
    }
}
