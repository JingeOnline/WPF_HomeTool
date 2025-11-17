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

        public WebImageModel WebImageModel { get; set; }

        public WebPageTabModel(string name)
        {
            Name = name;
            WebView = new WebView2();
            //WebView.Source = new Uri("http://www.google.com");
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
            await Task.Delay(2000);
            string html;
            try
            {
                string jsonResult = await WebView.ExecuteScriptAsync("document.documentElement.outerHTML");
                html = System.Text.Json.JsonSerializer.Deserialize<string>(jsonResult);
                Debug.WriteLine($"{Name} 导航结束: {uri} 抓取字节数：{html?.Length ?? 0}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{Name} 解析HTML出错: {ex.Message}");
                throw;
            }
            if (html.Contains("Welcome to the ImageFap protection system"))
            {
                throw new NotSupportedException("触发ImageFap防护系统，需要手动在浏览器中打开并通过验证");
            }
            return html;
            //return string.Empty;
        }

    }
}
