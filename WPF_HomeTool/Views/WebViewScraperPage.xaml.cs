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

        private async void StartMenuItem_Click(object sender, RoutedEventArgs e)
        {
            VM.StartTabControlScraper();
            await Task.Delay(1000);
            foreach (var item in WebPageTabControl.Items)
            {
                WebPageTabControl.SelectedItem = item;
                await Task.Delay(1000);
            }
            WebPageTabControl.SelectedIndex = 0;
            //await Task.Delay(1000);
            //WebPageTabControl.SelectedIndex= 0;
            //await Task.Delay(1000);
            //WebPageTabControl.SelectedIndex = 1;
            //await Task.Delay(1000);
            //WebPageTabControl.SelectedIndex = 0;
        }
    }
}
