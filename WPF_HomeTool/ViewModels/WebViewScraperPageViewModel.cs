using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using WPF_HomeTool.Helpers;
using WPF_HomeTool.Models;
using WPF_HomeTool.Services;
using static System.Net.WebRequestMethods;
using Visibility = System.Windows.Visibility;

namespace WPF_HomeTool.ViewModels
{
    public partial class WebViewScraperPageViewModel : ObservableObject
    {

        public event Action OnTabImageStarted;
        private readonly ILogger<WebViewScraperPageViewModel> _logger;

        [ObservableProperty]
        private string _webSite;
        [ObservableProperty]
        private ObservableCollection<WebPageTabModel> _WebPageTabModels=new ObservableCollection<WebPageTabModel>();
        [ObservableProperty]
        private ObservableCollection<WebAlbumModel> _WebAlbumModels=new ObservableCollection<WebAlbumModel>();
        [ObservableProperty]
        private Visibility _HeaderVisibility = Visibility.Visible;
        [ObservableProperty]
        private GridLength _DataGridLength = new GridLength(2, GridUnitType.Star);
        [ObservableProperty]
        private bool _isNeedCreateAlbumFolder;
        [ObservableProperty]
        private string _userInputAlbumUri;
        partial void OnIsNeedCreateAlbumFolderChanged(bool value)
        {
            ConfigHelper.WriteKeyValue("IsNeedCreateAlbumFolder", value ? "True" : "False");
        }
        [ObservableProperty]
        private string _imageSavePath;
        partial void OnImageSavePathChanged(string value)
        {
            ConfigHelper.WriteKeyValue("ImageSavePath", value);
        }
        private int UnCompletedCount;

        //private List<string> urls = new List<string>()
        //{
        //    只支持这种格式的页面
        //    "https://www.imagefap.com/photo/1187634730/?pgid=&gid=9068759&page=0",
        //    "https://www.imagefap.com/photo/1184537525/?pgid=&gid=9068759&page=0",
        //    "https://www.imagefap.com/photo/1152220050/?pgid=&gid=9068759&page=0",
        //    "https://www.imagefap.com/photo/1163688955/?pgid=&gid=9068759&page=0",
        //    "https://www.imagefap.com/photo/1757328285/?pgid=&gid=9068759&page=0",
        //    "https://www.imagefap.com/photo/1091188060/?pgid=&gid=9068759&page=0",
        //    "https://www.imagefap.com/photo/553653526/?pgid=&gid=9068759&page=0",
        //    "https://www.imagefap.com/photo/611135218/?pgid=&gid=9068759&page=0",
        //    这种格式的页面暂不支持，NavigationCompleted事件无法触发
        //    "https://www.imagefap.com/photo/1395360464/?pgid=&gid=13612474&page=0#0",
        //    "https://www.imagefap.com/photo/1395360464/?pgid=&gid=13612474&page=0#1",
        //    "https://www.imagefap.com/photo/1395360464/?pgid=&gid=13612474&page=0#2",
        //    "https://www.imagefap.com/photo/1395360464/?pgid=&gid=13612474&page=0#3",
        //    "https://www.imagefap.com/photo/1395360464/?pgid=&gid=13612474&page=0#4",
        //    "https://www.imagefap.com/photo/1395360464/?pgid=&gid=13612474&page=0#5",
        //    "https://www.imagefap.com/photo/1395360464/?pgid=&gid=13612474&page=0#6",
        //    "https://www.imagefap.com/photo/1395360464/?pgid=&gid=13612474&page=0#7",
        //    "https://www.imagefap.com/photo/1395360464/?pgid=&gid=13612474&page=0#8",
        //    "https://www.imagefap.com/photo/1395360464/?pgid=&gid=13612474&page=0#9",
        //};


        public WebViewScraperPageViewModel(ILogger<WebViewScraperPageViewModel> logger)
        {
            _logger = logger;
            IsNeedCreateAlbumFolder = ConfigHelper.ReadKeyValue("IsNeedCreateAlbumFolder") == "True";
            ImageSavePath = ConfigHelper.ReadKeyValue("ImagesSavePath")!;
        }
        [RelayCommand]
        private void SelectImagesSavePath()
        {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
            {
                dialog.IsFolderPicker = true; //Select Folder Only
                dialog.Multiselect = false;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    ImageSavePath = dialog.FileName;
                }
            }
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
        [RelayCommand]
        private void AddAlbumUri()
        {
            WebAlbumModels.Add(new WebAlbumModel(UserInputAlbumUri));
            UserInputAlbumUri = string.Empty;
        }
        public async Task StartTabControlScraper()
        {
            ImageFapService imageFapService = new ImageFapService();
            List<WebImageModel> webImageModels = await imageFapService.GetImagePageUrlFromAlbumPage(
                "https://www.imagefap.com/pictures/7132227/Maria");
            Queue<WebImageModel> webImageModelsQueue = new Queue<WebImageModel>(webImageModels);
            UnCompletedCount = webImageModelsQueue.Count;
            OnTabImageStarted?.Invoke();

            WebPageTabModels = new ObservableCollection<WebPageTabModel>()
            {
                new WebPageTabModel("Tab 1"),
                new WebPageTabModel("Tab 2"),
                new WebPageTabModel("Tab 3"),
            };
            Dictionary<Task<string>, WebPageTabModel> taskToWebPageTabModelDic = new Dictionary<Task<string>, WebPageTabModel>();

            foreach (var model in WebPageTabModels)
            {
                if (webImageModelsQueue.Count > 0)
                {
                    WebImageModel webImageModel = webImageModelsQueue.Dequeue();
                    model.WebImageModel = webImageModel;
                    taskToWebPageTabModelDic.Add(model.NavigateToUriAsync(webImageModel.PageUrl), model);
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
                    string imageUrl = await getImageUrlFromImagePage_ImageFap(html);
                    webPageTabModel.WebImageModel.ImageUrl = imageUrl;
                    Debug.WriteLine(webPageTabModel.Name + " 获取图片uri: " + imageUrl);
                    taskToWebPageTabModelDic.Remove(completedTask);
                    //WebImageModel imageModel = new WebImageModel() 
                    //{
                    //    ImageUrl=imageUrl,
                    //    FilePathWithoutExt= "F:\\Image Download\\"+Guid.NewGuid().ToString()
                    //};
                    await HttpHelper.DownloadWebImage(webPageTabModel.WebImageModel);
                    if (webImageModelsQueue.Count > 0)
                    {
                        WebImageModel webImageModel = webImageModelsQueue.Dequeue();
                        webPageTabModel.WebImageModel = webImageModel;
                        taskToWebPageTabModelDic.Add(webPageTabModel.NavigateToUriAsync(webImageModel.PageUrl), webPageTabModel);
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
                _logger.LogError(ex, $"无法在ImageFap图片页面找到图片的URL: {ex.Message}");
                _logger.LogInformation($"HTML内容:\r\n{html}");
            }
            return string.Empty;
            //string albumName = document.QuerySelector("title").InnerHtml;
            //string albumUrl = document.QuerySelector("link[rel=canonical]").GetAttribute("href");
        }
    }
}


