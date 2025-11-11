using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_HomeTool.Models;

namespace WPF_HomeTool.ViewModels
{
    public partial class WebViewScraperPageViewModel:ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<WebPageTabModel> _WebPageTabModels;
        [ObservableProperty]
        private Visibility _HeaderVisibility = Visibility.Visible;
        public WebViewScraperPageViewModel()
        {
            WebPageTabModels=new ObservableCollection<WebPageTabModel>()
            {
                new WebPageTabModel("Tab 1",new Uri("https://www.google.com")),
                new WebPageTabModel("Tab 2", new Uri("https://www.bing.com")),
                new WebPageTabModel("Tab 3", new Uri("https://www.bilibili.com")),
                new WebPageTabModel("Tab 4", new Uri("https://www.youtube.com")),
            };
        }

        public async void InitialWait()
        {
            await Task.Delay(5000);
        }

        [RelayCommand]
        private void WebTabExpand()
        {
            if(HeaderVisibility==Visibility.Visible)
            {
                HeaderVisibility=Visibility.Collapsed;
            }
            else
            {
                HeaderVisibility = Visibility.Visible;
            }
        }
    }
}
