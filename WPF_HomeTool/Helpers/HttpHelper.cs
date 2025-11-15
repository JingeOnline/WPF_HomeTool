using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WPF_HomeTool.Models;

namespace WPF_HomeTool.Helpers
{
    public class HttpHelper
    {

        public static async Task<string> GetHtmlContent(string url)
        {
            using var httpClient = new HttpClient();
            {
                try
                {
                    HttpResponseMessage response = await httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    string content = await response.Content.ReadAsStringAsync();
                    return content;
                }
                catch (Exception ex)
                {
                    Debug.Write("获取HTML内容失败 "+url+" : " + ex.ToString());
                    return string.Empty;
                }
            }
        }
        public static async Task DownloadWebImage(WebImageModel model)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            using var httpClient = new HttpClient();
            {
                Uri uri = new Uri(model.ImageUrl);
                //自动通过图片的URL获取图片的扩展名
                var uriWithoutQuery = uri.GetLeftPart(UriPartial.Path);
                var fileExtension = Path.GetExtension(uriWithoutQuery);
                var path = model.FilePathWithoutExt + fileExtension;
                try
                {
                    // Download the image and write to the file
                    var imageBytes = await httpClient.GetByteArrayAsync(uri);
                    File.WriteAllBytes(path, imageBytes);
                    model.DownloadStatus = WebImageDownloadStatus.Downloaded;
                    sw.Stop();
                    model.ImageDownloadTime = sw.Elapsed;
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
}
