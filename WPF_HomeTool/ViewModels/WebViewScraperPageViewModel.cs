using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        private ObservableCollection<WebPageTabModel> _WebPageTabModels = new ObservableCollection<WebPageTabModel>();
        [ObservableProperty]
        private ObservableCollection<WebAlbumModel> _WebAlbumModels = new ObservableCollection<WebAlbumModel>();
        [ObservableProperty]
        private ObservableCollection<WebImageModel> _WebImageModels = new ObservableCollection<WebImageModel>();
        [ObservableProperty]
        private WebAlbumModel _selectedWebAlbumModel;
        [ObservableProperty]
        private Visibility _HeaderVisibility = Visibility.Visible;
        [ObservableProperty]
        private GridLength _DataGridLength = new GridLength(2, GridUnitType.Star);
        [ObservableProperty]
        private bool _isNeedCreateAlbumFolder;
        [ObservableProperty]
        private string _userInputAlbumUri;
        [ObservableProperty]
        private string _statusText = String.Empty;
        [ObservableProperty]
        private string _statusDetailText = String.Empty;

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
        public WebViewScraperPageViewModel(ILogger<WebViewScraperPageViewModel> logger)
        {
            _logger = logger;
            IsNeedCreateAlbumFolder = ConfigHelper.ReadKeyValue("IsNeedCreateAlbumFolder") == "True";
            ImageSavePath = ConfigHelper.ReadKeyValue("ImageSavePath")!;
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
        private async void AddAlbumUri()
        {
            if (Uri.IsWellFormedUriString(UserInputAlbumUri, UriKind.Absolute))
            {
                WebAlbumModel model = new WebAlbumModel(UserInputAlbumUri);
                WebAlbumModels.Add(model);
                UserInputAlbumUri = string.Empty;
                await Task.Run(async () =>
                {
                    try
                    {
                        model = await (new ImageFapService(ImageSavePath,IsNeedCreateAlbumFolder)).GetImagePagesFromWebAlbumModel(model);
                    }
                    catch (Exception ex)
                    {
                        DebugAndOutputToStatusbar($"获取相册页面中的图片链接失败: {model.AlbumUrl}");
                        _logger.LogError(ex, $"获取相册页面中的图片链接失败: {model.AlbumUrl}");
                        model.TotalImageCount = -1;
                    }
                });
                if (model.TotalImageCount > 0)
                {
                    foreach (var imageModel in model.WebImageModelList)
                    {
                        WebImageModels.Add(imageModel);
                    }
                }
            }
        }
        [RelayCommand]
        private void RemoveSelectedAlbum()
        {
            //删除选中的相册中包含的图片
            if (SelectedWebAlbumModel != null && SelectedWebAlbumModel.WebImageModelList != null)
            {
                foreach (var imageModel in SelectedWebAlbumModel.WebImageModelList)
                {
                    WebImageModels.Remove(imageModel);
                }
            }
            //删除选中的相册
            if (SelectedWebAlbumModel != null)
            {
                WebAlbumModels.Remove(SelectedWebAlbumModel);
            }
        }
        [RelayCommand]
        private async void Clear()
        {
            WebAlbumModels.Clear();
            WebImageModels.Clear();
        }
        public async Task StartTabControlScraper()
        {

            WebPageTabModels = new ObservableCollection<WebPageTabModel>()
            {
                new WebPageTabModel("Tab 1"),
                new WebPageTabModel("Tab 2"),
                new WebPageTabModel("Tab 3"),
                new WebPageTabModel("Tab 4"),
                new WebPageTabModel("Tab 5"),
            };
            Dictionary<Task<string>, WebPageTabModel> taskToWebPageTabModelDic = new Dictionary<Task<string>, WebPageTabModel>();

            int index = 0;
            foreach (var webPageTabModel in WebPageTabModels)
            {
                if (WebImageModels.Count > index)
                {
                    WebImageModel webImageModel = WebImageModels[index];
                    webImageModel.DownloadStatus = WebImageDownloadStatus.Downloading;
                    webPageTabModel.WebImageModel = webImageModel;
                    taskToWebPageTabModelDic.Add(webPageTabModel.NavigateToUriAsync(webImageModel.PageUrl), webPageTabModel);
                    index++;
                }
                else
                {
                    break;
                }
            }
            OnTabImageStarted?.Invoke();
            while (WebImageModels.Count(x => x.DownloadStatus == WebImageDownloadStatus.Downloading) > 0)
            {
                try
                {
                    Task<string> completedTask = await Task.WhenAny(taskToWebPageTabModelDic.Keys);
                    string html = await completedTask;
                    WebPageTabModel webPageTabModel = taskToWebPageTabModelDic[completedTask];
                    string imageUrl = await getImageUrlFromImagePage_ImageFap(html);
                    webPageTabModel.WebImageModel.ImageUrl = imageUrl;
                    DebugAndOutputToStatusbar(webPageTabModel.Name + " 成功获取到图片uri: " + imageUrl);
                    taskToWebPageTabModelDic.Remove(completedTask);
                    await HttpHelper.DownloadWebImage(webPageTabModel.WebImageModel);
                    if (WebImageModels.Count(x => x.DownloadStatus == WebImageDownloadStatus.UnDownload) > 0)
                    {
                        WebImageModel webImageModel = WebImageModels[index];
                        webImageModel.DownloadStatus = WebImageDownloadStatus.Downloading;
                        webPageTabModel.WebImageModel = webImageModel;
                        taskToWebPageTabModelDic.Add(webPageTabModel.NavigateToUriAsync(webImageModel.PageUrl), webPageTabModel);
                        index++;
                    }
                    //UnCompletedCount--;
                }
                catch (Exception e)
                {
                    DebugAndOutputToStatusbar($"Error: {e.Message}");
                    _logger.LogError(e, "TabControl中解析页面中的图片链接发生异常");
                }
            }
            await Task.WhenAll(taskToWebPageTabModelDic.Keys);
            DebugAndOutputToStatusbar("All tasks completed.");
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
                DebugAndOutputToStatusbar($"无法在ImageFap图片页面找到图片的URL: {ex.Message}");
                _logger.LogError(ex, $"无法在ImageFap图片页面找到图片的URL: {ex.Message}");
                _logger.LogInformation($"HTML内容:\r\n{html}");
            }
            return string.Empty;
            //string albumName = document.QuerySelector("title").InnerHtml;
            //string albumUrl = document.QuerySelector("link[rel=canonical]").GetAttribute("href");
        }

        private void DebugAndOutputToStatusbar(string s)
        {
            Debug.WriteLine(s);
            StatusText = s;
            StatusDetailText += s+Environment.NewLine;
        }

    }
}


