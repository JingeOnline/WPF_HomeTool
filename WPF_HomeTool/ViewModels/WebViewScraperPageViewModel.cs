using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WPF_HomeTool.Helpers;
using WPF_HomeTool.Models;
using static System.Net.WebRequestMethods;
using Visibility = System.Windows.Visibility;

namespace WPF_HomeTool.ViewModels
{
    public partial class WebViewScraperPageViewModel : ObservableObject
    {
        private readonly ILogger<WebViewScraperPageViewModel> _logger;

        [ObservableProperty]
        private ObservableCollection<WebPageTabModel> _WebPageTabModels;
        [ObservableProperty]
        private Visibility _HeaderVisibility = Visibility.Visible;
        [ObservableProperty]
        private GridLength _DataGridLength = new GridLength(2, GridUnitType.Star);

        private int UnCompletedCount;

        private List<string> urls = new List<string>()
        {
            "https://www.imagefap.com/photo/1187634730/?pgid=&gid=9068759&page=0",
            "https://www.imagefap.com/photo/1184537525/?pgid=&gid=9068759&page=0",
            "https://www.imagefap.com/photo/1152220050/?pgid=&gid=9068759&page=0",
            "https://www.imagefap.com/photo/1163688955/?pgid=&gid=9068759&page=0",
            "https://www.imagefap.com/photo/1757328285/?pgid=&gid=9068759&page=0",
            "https://www.imagefap.com/photo/1091188060/?pgid=&gid=9068759&page=0",
            "https://www.imagefap.com/photo/553653526/?pgid=&gid=9068759&page=0",
            "https://www.imagefap.com/photo/611135218/?pgid=&gid=9068759&page=0",
            //"https://www.imagefap.com/photo/1395360464/?pgid=&gid=13612474&page=0#0",
            //"https://www.imagefap.com/photo/1395360464/?pgid=&gid=13612474&page=0#1",
            //"https://www.imagefap.com/photo/1395360464/?pgid=&gid=13612474&page=0#2",
            //"https://www.imagefap.com/photo/1395360464/?pgid=&gid=13612474&page=0#3",
            //"https://www.imagefap.com/photo/1395360464/?pgid=&gid=13612474&page=0#4",
            //"https://www.imagefap.com/photo/1395360464/?pgid=&gid=13612474&page=0#5",
            //"https://www.imagefap.com/photo/1395360464/?pgid=&gid=13612474&page=0#6",
            //"https://www.imagefap.com/photo/1395360464/?pgid=&gid=13612474&page=0#7",
            //"https://www.imagefap.com/photo/1395360464/?pgid=&gid=13612474&page=0#8",
            //"https://www.imagefap.com/photo/1395360464/?pgid=&gid=13612474&page=0#9",
            //"https://www.imagefap.com/photo/1264451856/?pgid=&gid=13612474&page=0",
            //"https://www.imagefap.com/photo/230603319/?pgid=&gid=13612474&page=0",
            //"https://www.imagefap.com/photo/1395360464/?pgid=&gid=13612474&page=0",
            //"https://www.imagefap.com/photo/241832992/?pgid=&gid=13612474&page=0"
            //"https://www.google.com",
            //"https://www.youtube.com",
            //"https://www.bing.com",
            //"https://www.baidu.com",
            //"https://www.github.com",
            //"https://www.stackoverflow.com",
            //"https://www.reddit.com",
            //"https://www.wikipedia.org",
            //"https://www.twitter.com",
            //"https://www.facebook.com",
            //"https://www.imagefap.com/photo/618948872/?pgid=&gid=13610427&page=0",
            //"https://www.imagefap.com/photo/618948872/?pgid=&gid=13610427&page=0#1",
            //"https://www.imagefap.com/photo/618948872/?pgid=&gid=13610427&page=0#2",
            //"https://www.imagefap.com/photo/618948872/?pgid=&gid=13610427&page=0#3",
            //"https://www.imagefap.com/photo/618948872/?pgid=&gid=13610427&page=0#4",
            //"https://www.imagefap.com/photo/618948872/?pgid=&gid=13610427&page=0#5",
            //"https://www.imagefap.com/photo/618948872/?pgid=&gid=13610427&page=0#6",
        };

        private Queue<string> urlsQueue;

        public WebViewScraperPageViewModel(ILogger<WebViewScraperPageViewModel> logger)
        {
            urlsQueue = new Queue<string>(urls);
            UnCompletedCount = urlsQueue.Count;
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
            Dictionary<Task<string>, WebPageTabModel> taskToWebPageTabModelDic = new Dictionary<Task<string>, WebPageTabModel>();

            //List<Task<string>> tasks = new List<Task<string>>();
            foreach (var model in WebPageTabModels)
            {
                if (urlsQueue.Count > 0)
                {
                    taskToWebPageTabModelDic.Add(model.NavigateToUriAsync(urlsQueue.Dequeue()), model);
                    //tasks.Add(model.NavigateToUriAsync(urlsQueue.Dequeue()));
                }
                else
                {
                    break;
                }
            }
            while (UnCompletedCount > 0)
            {
                try
                {
                    Task<string> completedTask = await Task.WhenAny(taskToWebPageTabModelDic.Keys);
                    string html = await completedTask;
                    WebPageTabModel webPageTabModel = taskToWebPageTabModelDic[completedTask];
                    string imageUrl=await getImageUrlFromImagePage_ImageFap(html);
                    Debug.WriteLine(webPageTabModel.Name + " 获取图片uri: " + imageUrl);
                    taskToWebPageTabModelDic.Remove(completedTask);
                    WebImageModel imageModel = new WebImageModel() 
                    {
                        ImageUrl=imageUrl,
                        FilePathWithoutExt= "F:\\Image Download\\"+Guid.NewGuid().ToString()
                    };
                    await FileHelper.WebImageDownload(imageModel);
                    if (urlsQueue.Count > 0)
                    {
                        taskToWebPageTabModelDic.Add(webPageTabModel.NavigateToUriAsync(urlsQueue.Dequeue()), webPageTabModel);
                    }
                    UnCompletedCount--;
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Error: {e.Message}");
                }
            }
            await Task.WhenAll(taskToWebPageTabModelDic.Keys);
            Debug.WriteLine("All tasks completed.");
        }
        private async Task<string> getImageUrlFromImagePage_ImageFap(string html)
        {
            try
            {
                IConfiguration config = Configuration.Default;
                IBrowsingContext context = BrowsingContext.New(config);
                IDocument document = await context.OpenAsync(req => req.Content(html));
                IElement imageElement = document.QuerySelector("div.image-wrapper>span>img");
                string pageUrl = imageElement.GetAttribute("src");
                return pageUrl;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"无法在ImageFap图片页面找到图片的URL: {ex.Message}");
                _logger.LogError(ex,$"无法在ImageFap图片页面找到图片的URL: {ex.Message}");
                _logger.LogInformation($"HTML内容:\r\n{html}");
            }
            return string.Empty;
            //string albumName = document.QuerySelector("title").InnerHtml;
            //string albumUrl = document.QuerySelector("link[rel=canonical]").GetAttribute("href");
        }
    }
}


