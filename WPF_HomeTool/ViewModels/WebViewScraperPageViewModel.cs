using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_HomeTool.Models;

namespace WPF_HomeTool.ViewModels
{
    public partial class WebViewScraperPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<WebPageTabModel> _WebPageTabModels;
        [ObservableProperty]
        private Visibility _HeaderVisibility = Visibility.Visible;
        [ObservableProperty]
        private GridLength _DataGridLength = new GridLength(2, GridUnitType.Star);

        //private int UnCompletedCount;

        private List<string> urls = new List<string>()
        {
            "https://www.google.com",
            "https://www.youtube.com",
            "https://www.bing.com",
            "https://www.baidu.com",
            "https://www.github.com",
            "https://www.stackoverflow.com",
            "https://www.reddit.com",
            "https://www.wikipedia.org",
            "https://www.twitter.com",
            "https://www.facebook.com",
        };

        private Queue<string> urlsQueue;

        public WebViewScraperPageViewModel()
        {
            urlsQueue = new Queue<string>(urls);
            //UnCompletedCount = urlsQueue.Count;
        }

        [RelayCommand]
        private void WebTabExpand()
        {
            if (HeaderVisibility == Visibility.Visible)
            {
                DataGridLength = new GridLength(0);
                HeaderVisibility = Visibility.Collapsed;
            }
            else
            {
                DataGridLength = new GridLength(2, GridUnitType.Star);
                HeaderVisibility = Visibility.Visible;
            }
        }

        public async Task StartTabControlScraper()
        {
            WebPageTabModels = new ObservableCollection<WebPageTabModel>()
            {
                new WebPageTabModel("Tab 1"),
                new WebPageTabModel("Tab 2"),
                new WebPageTabModel("Tab 3"),
            };

            List<Task<string>> tasks = new List<Task<string>>();
            foreach (var model in WebPageTabModels)
            {
                tasks.Add(model.NavigateToUriAsync(urlsQueue.Dequeue()));
            }
            while (urlsQueue.Count > 0)
            {
                try
                {
                    Task<string> completedTask = await Task.WhenAny(tasks);
                    //string html = await completedTask;
                    int index = tasks.IndexOf(completedTask);
                    //Debug.WriteLine($"{WebPageTabModels[index].Name} fetched {html?.Length ?? 0} characters in {WebPageTabModels[index].WebView.Source}.");
                    tasks[index] = WebPageTabModels[index].NavigateToUriAsync(urlsQueue.Dequeue());
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Error: {e.Message}");
                }
            }
            await Task.WhenAll(tasks);
            Debug.WriteLine("All tasks completed.");
        }
    }

    //string html1;
    //string html2;
    //WebPageTabModel model1 = new WebPageTabModel("Tab 1", new Uri("https://www.google.com"));
    //WebPageTabModel model2 = new WebPageTabModel("Tab 2", new Uri("https://www.youtube.com"));

    //model1.WebView.NavigationCompleted += async (o, e) =>
    //{
    //    html1 = await model1.WebView.ExecuteScriptAsync("document.documentElement.outerHTML;");
    //    Debug.WriteLine(html1.Length);
    //};

    //model2.WebView.NavigationCompleted += async (o, e) =>
    //{
    //    html2 = await model2.WebView.ExecuteScriptAsync("document.documentElement.outerHTML;");
    //    Debug.WriteLine(html2.Length);
    //};


    //WebPageTabModels = new ObservableCollection<WebPageTabModel>() { model1, model2 };
    //await Task.Delay(10000);
    //model1.WebView.CoreWebView2.Navigate("https://www.baidu.com");
    //model2.WebView.CoreWebView2.Navigate("https://www.bilibili.com");
    //await Task.Delay(10000);
    //model1.WebView.CoreWebView2.Navigate("https://www.github.com");
    //model2.WebView.CoreWebView2.Navigate("https://www.github.com");
}

