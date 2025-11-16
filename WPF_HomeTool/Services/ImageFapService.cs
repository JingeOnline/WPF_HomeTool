using AngleSharp;
using AngleSharp.Dom;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_HomeTool.Helpers;
using WPF_HomeTool.Models;

namespace WPF_HomeTool.Services
{
    public class ImageFapService
    {
        private const string BaseURL = "https://www.imagefap.com";
        private string _downloadFolderPath;
        private bool _isCreateSubFolder;
        //private string _albumName;
        private readonly NLog.Logger _logger= NLog.LogManager.GetCurrentClassLogger();
        public ImageFapService(string downloadFolderPath, bool isCreateSubFolder)
        {
            _downloadFolderPath = downloadFolderPath;
            _isCreateSubFolder = isCreateSubFolder;
            //DownloadFolderPath = "F:\\Image Download\\test";
        }

        public async Task<WebAlbumModel> GetImagePagesFromWebAlbumModel(WebAlbumModel webAlbumModel)
        {
            webAlbumModel.WebImageModelList = await GetImagePageUrlFromAlbumPage(webAlbumModel.AlbumUrl);
            webAlbumModel.TotalImageCount = webAlbumModel.WebImageModelList.Count;
            if (webAlbumModel.TotalImageCount > 0)
            {
                webAlbumModel.AlbumName = webAlbumModel.WebImageModelList.First().AlbumName;
            }
            return webAlbumModel;
        }
        public async Task<List<WebImageModel>> GetImagePageUrlFromAlbumPage(string albumPageUrl)
        {
            var config = Configuration.Default.WithDefaultLoader();
            IBrowsingContext context = BrowsingContext.New(config);
            List<WebImageModel> models = await getImagePageUrlFromAlbumPage(context, albumPageUrl);
            return models;
        }
        private async Task<List<WebImageModel>> getImagePageUrlFromAlbumPage(IBrowsingContext context, string albumPageUrl,
            string pageIndexUrl = null, string albumName = null, int index = 1)
        {
            try
            {
                List<WebImageModel> list = new List<WebImageModel>();
                IDocument document;
                if (pageIndexUrl == null)
                {
                    string html = await HttpHelper.GetHtmlContent(albumPageUrl);
                    document = await context.OpenAsync(request => request.Content(html));
                    albumName = document.QuerySelector("title").InnerHtml;
                    albumName = FileHelper.ReplaceWindowsReservedChar(albumName);
                    //相册第一页的地址不是唯一的，所以需要这里获取真正的页面URL
                    albumPageUrl = document.QuerySelector("link[rel=canonical]").GetAttribute("href");
                }
                else
                {
                    string html = await HttpHelper.GetHtmlContent(albumPageUrl + pageIndexUrl);
                    document = await context.OpenAsync(request => request.Content(html));
                }
                var cells = document.QuerySelectorAll("img._lazy");
                foreach (var img in cells)
                {
                    string url = BaseURL + img.ParentElement.GetAttribute("href");
                    string fileDirPath=_isCreateSubFolder?_downloadFolderPath+"\\"+albumName:_downloadFolderPath;
                    WebImageModel model = new WebImageModel()
                    {
                        AlbumUrl = albumPageUrl,
                        AlbumName = albumName,
                        IndexInAlbum = index,
                        DownloadStatus = WebImageDownloadStatus.UnDownload,
                        //ToString("N")表示32位无连字符的数字，默认情况下Guid包含连字符“-”，总长度36位
                        FilePathWithoutExt = fileDirPath + "\\" + albumName + "_" + index + " "
                            + Guid.NewGuid().ToString("N").Substring(0, 8),
                        PageUrl = url,
                    };
                    list.Add(model);
                    index++;
                }

                var pageIndexElements = document.QuerySelectorAll("#gallery span>a");
                foreach (var element in pageIndexElements)
                {
                    if (element.InnerHtml == ":: next ::")
                    {
                        pageIndexUrl = element.GetAttribute("href");
                        //Console.WriteLine(indexUrl);
                        list.AddRange(await getImagePageUrlFromAlbumPage(context, albumPageUrl, pageIndexUrl, albumName, index));
                        break;
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                _logger.Error(ex, "在ImageFap获取相册页面中的图片链接时发生异常");
                throw;
            }
        }
    }
}
