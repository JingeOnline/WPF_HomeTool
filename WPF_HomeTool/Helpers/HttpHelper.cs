using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WPF_HomeTool.Models;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WPF_HomeTool.Helpers
{
    public class HttpHelper
    {
        public static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        //为 HttpClient 加 “浏览器指纹” Header，imagefap 对非浏览器 TLS + UA 会直接丢包
        private static HttpClientHandler handler = new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.All
        };

        private static HttpClient httpClient;
        public HttpHelper()
        {


        }

        public static async Task<string> GetHtmlContent(string url)
        {
            if (httpClient == null)
            {
                httpClient = new HttpClient(handler);
                //为 HttpClient 加 “浏览器指纹” Header，imagefap 对非浏览器 TLS + UA 会直接丢包
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0 Safari/537.36"
                );
                httpClient.DefaultRequestHeaders.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
                httpClient.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-US,en;q=0.9");
            }
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string content = await response.Content.ReadAsStringAsync();
                return content;
            }
            catch (Exception ex)
            {
                Debug.Write("获取HTML内容失败 " + url + " : " + ex.ToString());
                _logger.Error(ex, "获取HTML内容失败 " + url);
                throw;
                //return string.Empty;
            }
        }
        public static async Task DownloadWebImage(WebImageModel model, Action<WebImageModel> OnDownloadSucceed)
        {
            if (httpClient == null)
            {
                httpClient = new HttpClient(handler);
                //为 HttpClient 加 “浏览器指纹” Header，imagefap 对非浏览器 TLS + UA 会直接丢包
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0 Safari/537.36"
                );
                httpClient.DefaultRequestHeaders.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
                httpClient.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-US,en;q=0.9");
            }
            Stopwatch sw = new Stopwatch();
            sw.Start();

            Uri uri = new Uri(model.ImageUrl);
            //自动通过图片的URL获取图片的扩展名
            var uriWithoutQuery = uri.GetLeftPart(UriPartial.Path);
            var fileExtension = Path.GetExtension(uriWithoutQuery);
            var path = model.FilePathWithoutExt + fileExtension;
            try
            {
                // Download the image and write to the file
                var imageBytes = await httpClient.GetByteArrayAsync(uri);
                //如果文件夹不存在，则创建文件夹
                string dir = Path.GetDirectoryName(path);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                await File.WriteAllBytesAsync(path, imageBytes);
                model.DownloadStatus = WebImageDownloadStatus.Downloaded;
                sw.Stop();
                model.ImageDownloadTime = sw.Elapsed;
                OnDownloadSucceed?.Invoke(model);
                if (sw.Elapsed.TotalSeconds > 3.0)
                {
                    Debug.WriteLine($"{sw.Elapsed.TotalSeconds}秒，下载{uri}共消耗时间");
                }
            }
            catch (Exception ex)
            {
                Debug.Write(model.ImageUrl + "下载失败，已跳过：" + ex.ToString());
                //下载失败时，跳过当前图片，去下载下一张图片。当前图片不会被保存到进度文件中。
                model.DownloadStatus = WebImageDownloadStatus.Failed;
                return;
            }

        }
    }
}
