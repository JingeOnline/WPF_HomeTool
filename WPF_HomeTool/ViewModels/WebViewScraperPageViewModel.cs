using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
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
        private string _userInputAlbumUri;
        [ObservableProperty]
        private string _statusText = String.Empty;
        [ObservableProperty]
        private string _statusDetailText = String.Empty;
        [ObservableProperty]
        private int _tabAmount = 4;
        partial void OnTabAmountChanged(int oldValue, int newValue)
        {
            if (oldValue != newValue)
            {
                ConfigHelper.WriteKeyValue("ParallelTabAmount", newValue.ToString());
            }
        }
        [ObservableProperty]
        private ObservableCollection<int> _tabAmountCollection;
        [ObservableProperty]
        private bool _isNeedCreateAlbumFolder;

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

        [ObservableProperty]
        private bool _isHumanValided;
        [ObservableProperty]
        private bool _isNeedHumandValidate;
        public WebViewScraperPageViewModel(ILogger<WebViewScraperPageViewModel> logger)
        {
            _logger = logger;
            IsNeedCreateAlbumFolder = ConfigHelper.ReadKeyValue("IsNeedCreateAlbumFolder") == "True";
            ImageSavePath = ConfigHelper.ReadKeyValue("ImageSavePath")!;
            TabAmount = int.Parse(ConfigHelper.ReadKeyValue("ParallelTabAmount")!);
            TabAmountCollection = new ObservableCollection<int>() { 1, 2, 3, 4, 5, 6, 8, 10 };
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
                ImageFapService imageFapService = new ImageFapService(ImageSavePath, IsNeedCreateAlbumFolder);
                UserInputAlbumUri = UserInputAlbumUri.Trim();
                try
                {
                    if (imageFapService.IsAlbumUrlAlreadyDownloaded(UserInputAlbumUri))
                    {
                        DebugAndOutputToStatusbar($"相册地址已存在于历史记录中，跳过: {UserInputAlbumUri}");
                        UserInputAlbumUri = string.Empty;
                        return;
                    }
                }
                catch (Exception ex)
                {
                    DebugAndOutputToStatusbar($"检查相册地址是否存在于历史记录时发生错误: {ex.Message}");
                    _logger.LogError(ex, "检查相册地址是否存在于历史记录时发生错误");
                    return;
                }

                WebAlbumModel model = new WebAlbumModel(UserInputAlbumUri);
                WebAlbumModels.Add(model);
                UserInputAlbumUri = string.Empty;

                await Task.Run(async () =>
                {
                    try
                    {
                        model = await imageFapService.GetImagePagesFromWebAlbumModel(model);
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
        [RelayCommand]
        private void LoadUndwonloadWebImageModelsFromSave()
        {
            ImageFapService imageFapService = new ImageFapService(ImageSavePath, IsNeedCreateAlbumFolder);
            IEnumerable<WebImageModel> models = imageFapService.LoadUnDownloadFromSave();
            if (models != null && models.Count() > 0)
            {
                foreach (WebImageModel model in models)
                {
                    WebImageModels.Add(model);
                }
            }
        }
        [RelayCommand]
        private void OpenSaveFolderInFileExplorer()
        {
            if (!string.IsNullOrEmpty(ImageSavePath) && Directory.Exists(ImageSavePath))
            {
                string windowsFormatPath = Path.GetFullPath(ImageSavePath);
                System.Diagnostics.Process.Start("explorer.exe", windowsFormatPath);
            }
        }
        [RelayCommand]
        private void HumandValidateCompleted()
        {
            IsHumanValided = true;
            IsNeedHumandValidate = false;
        }
        public async Task StartTabControlScraper()
        {
            DebugAndOutputToStatusbar("开始使用TabControl下载页面中的图片...");
            IEnumerable<int> tabsIndex = Enumerable.Range(1, TabAmount);
            foreach (var i in tabsIndex)
            {
                WebPageTabModels.Add(new WebPageTabModel($"Tab {i}"));
            }
            //WebPageTabModels = new ObservableCollection<WebPageTabModel>()
            //{
            //    new WebPageTabModel("Tab 1"),
            //    new WebPageTabModel("Tab 2"),
            //    new WebPageTabModel("Tab 3"),
            //    new WebPageTabModel("Tab 4"),
            //    new WebPageTabModel("Tab 5"),
            //};
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
                if (taskToWebPageTabModelDic.Count == 0)
                {
                    break;
                }
                Task<string> completedTask = await Task.WhenAny(taskToWebPageTabModelDic.Keys);
                WebPageTabModel webPageTabModel = taskToWebPageTabModelDic[completedTask];
                try
                {
                    string html = await completedTask;//导航如果失败，会从这里抛出异常
                    string imageUrl = await getImageUrlFromImagePage_ImageFap(html);
                    webPageTabModel.WebImageModel.ImageUrl = imageUrl;
                    DebugAndOutputToStatusbar(webPageTabModel.Name + " 成功获取到图片uri: " + imageUrl);
                    taskToWebPageTabModelDic.Remove(completedTask);
                    //异步下载图片，速度快，但是容易触发人机验证防护
                    HttpHelper.DownloadWebImage(webPageTabModel.WebImageModel, ImageFapService.RemoveDownloadedFromSave);
                    //同步下载图片，速度慢，但是不容易触发人机验证防护
                    //await HttpHelper.DownloadWebImage(webPageTabModel.WebImageModel, ImageFapService.RemoveDownloadedFromSave);
                }
                catch (NotSupportedException nse)
                {
                    DebugAndOutputToStatusbar(nse.Message);
                    _logger.LogError(nse.Message);
                    if (IsHumanValided)
                    {
                        IsHumanValided = false;
                    }
                    IsNeedHumandValidate= true;
                    while (!IsHumanValided)
                    {
                        await Task.Delay(200);
                    }
                    taskToWebPageTabModelDic.Remove(completedTask);
                }
                catch (Exception e)
                {
                    DebugAndOutputToStatusbar($"Error: {e.Message}");
                    _logger.LogError(e, "TabControl中解析页面中的图片链接发生异常");
                    taskToWebPageTabModelDic.Remove(completedTask);
                }
                //即使上面发生异常，通过try catch把旧的任务移除，继续添加新的任务
                if (WebImageModels.Count(x => x.DownloadStatus == WebImageDownloadStatus.UnDownload) > 0)
                {
                    WebImageModel webImageModel = WebImageModels[index];
                    webImageModel.DownloadStatus = WebImageDownloadStatus.Downloading;
                    webPageTabModel.WebImageModel = webImageModel;
                    taskToWebPageTabModelDic.Add(webPageTabModel.NavigateToUriAsync(webImageModel.PageUrl), webPageTabModel);
                    index++;
                }

            }
            await Task.WhenAll(taskToWebPageTabModelDic.Keys);
            DebugAndOutputToStatusbar("All tasks completed.");
            WebPageTabModels.Clear();
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
                throw new Exception("无法在ImageFap图片页面找到图片的URL", ex);
            }
            //return string.Empty;
            //string albumName = document.QuerySelector("title").InnerHtml;
            //string albumUrl = document.QuerySelector("link[rel=canonical]").GetAttribute("href");
        }

        private void DebugAndOutputToStatusbar(string s)
        {
            Debug.WriteLine(s);
            StatusText = s;
            StatusDetailText += s + Environment.NewLine;
        }

    }
}


