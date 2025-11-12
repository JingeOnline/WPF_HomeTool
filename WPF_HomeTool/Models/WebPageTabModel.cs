using Microsoft.AspNetCore.Mvc;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_HomeTool.Models
{
    public partial class WebPageTabModel : ObservableObject
    {
        [ObservableProperty]
        private string _Name;
        [ObservableProperty]
        private WebView2 _WebView;
        [ObservableProperty]
        private bool _IsNavigateComplete = false;
        public WebPageTabModel(string name)
        {
            Name = name;
            WebView = new WebView2();
            WebView.Source = new Uri("http://www.google.com");
            //WebView.Source = uri;
            //WebView.Initialized += (o, e) => { Debug.WriteLine($"{name} is intialized"); };
            //WebView.CoreWebView2InitializationCompleted += (o, e) => { Debug.WriteLine($"{name} is CoreWebView2InitializationCompleted"); };
            //Debug.WriteLine("Initial "+uri);
            //InitializeWebView(uri);
            WebView.NavigationCompleted += (o, e) =>
            {
                IsNavigateComplete = true;
            };
        }

        public async Task<string> NavigateToUriAsync(string uri)
        {
            Debug.WriteLine($"{Name} 开始导航: {uri}");
            IsNavigateComplete = false;
            await WebView.EnsureCoreWebView2Async();
            WebView.CoreWebView2.Navigate(uri);
            while (!IsNavigateComplete)
            {
                await Task.Delay(100);
            }
            await Task.Delay(1000);
            string jsonResult = await WebView.ExecuteScriptAsync("document.documentElement.outerHTML");
            string html = System.Text.Json.JsonSerializer.Deserialize<string>(jsonResult);
            Debug.WriteLine($"{Name} 导航结束: {uri} 抓取字节数：{html?.Length ?? 0}");
            return html;
        }

        //private async void InitializeWebView(Uri uri)
        //{
        //    await WebView.EnsureCoreWebView2Async();
        //    WebView.Source = uri;
        //    //Debug.WriteLine("Initial " + uri);
        //}

    }
}
