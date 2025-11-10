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
    /// Interaction logic for WebViewScraperPage.xaml
    /// </summary>
    public partial class WebViewScraperPage : Page
    {
        public WebViewScraperPageViewModel VM { get; }
        public WebViewScraperPage(WebViewScraperPageViewModel vm)
        {
            VM = vm;
            DataContext = this;
            InitializeComponent();
        }

        private void WebTabExpandButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
