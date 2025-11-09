using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_HomeTool.Controls;
using WPF_HomeTool.Models;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;

namespace WPF_HomeTool.Helpers
{
    public class FileHelper
    {
        /// <summary>
        /// 返回带单位的文件大小
        /// </summary>
        /// <param name="sizeInBytes">文件大小，单位Bytes</param>
        /// <returns></returns>
        public static string ConvertBytesToHumanReadable(long sizeInBytes)
        {
            const long KB = 1024;
            const long MB = KB * 1024;
            const long GB = MB * 1024;
            const long TB = GB * 1024;

            if (sizeInBytes >= TB)
            {
                return $"{sizeInBytes / (double)TB:F2} TB";
            }
            else if (sizeInBytes >= GB)
            {
                return $"{sizeInBytes / (double)GB:F2} GB";
            }
            else if (sizeInBytes >= MB)
            {
                return $"{sizeInBytes / (double)MB:F2} MB";
            }
            else if (sizeInBytes >= KB)
            {
                return $"{sizeInBytes / (double)KB:F2} KB";
            }
            else
            {
                return $"{sizeInBytes} Bytes";
            }
        }

        /// <summary>
        /// 返回目录下的所有后代文件（递归的）
        /// </summary>
        /// <param name="dirInfo"></param>
        /// <returns></returns>
        public static List<FileInfo> GetAllFilesRecursively(DirectoryInfo dirInfo)
        {
            try
            {
                // Use SearchOption.AllDirectories to include all subdirectories
                return dirInfo.GetFiles("*.*", SearchOption.AllDirectories).ToList();
            }
            catch (UnauthorizedAccessException ex)
            {
                ModernMessageBox.Show(dirInfo.FullName + Environment.NewLine + ex.Message,
                    "Access Denied", Controls.MessageBoxButton.OK, Controls.MessageBoxImage.Warning);
                return new List<FileInfo>();
            }
            catch (DirectoryNotFoundException ex)
            {
                ModernMessageBox.Show(dirInfo.FullName + Environment.NewLine + ex.Message,
                        "Directory not found", Controls.MessageBoxButton.OK, Controls.MessageBoxImage.Warning);
                return new List<FileInfo>();
            }
            catch (Exception ex)
            {
                ModernMessageBox.Show(dirInfo.FullName + Environment.NewLine + ex.Message,
                        "Error In GetAllFilesRecursively", Controls.MessageBoxButton.OK, Controls.MessageBoxImage.Warning);
                return new List<FileInfo>();
            }
        }

        /// <summary>
        /// 统计文件的类型，输出字符串。包含文件总数，和每种类型文件的数量。
        /// </summary>
        /// <param name="fileInfos"></param>
        /// <returns></returns>
        public static string GetDirectoryFileExtCountString(IEnumerable<FileInfo> fileInfos)
        {
            List<string> exts = new List<string>();
            foreach (var file in fileInfos)
            {
                string? ext = file.Extension;
                if (string.IsNullOrEmpty(ext))
                {
                    exts.Add("空");
                }
                else
                {
                    exts.Add(ext.Substring(1).ToUpper());
                }
            }
            IEnumerable<FileExtCountModel> fileExtCounts = from ext in exts
                                                           group ext by ext into g
                                                           select new FileExtCountModel(g.Key, g.Count());

            StringBuilder sb = new StringBuilder();
            sb.Append($"Total: {fileInfos.Count()}");
            foreach (var item in fileExtCounts)
            {
                sb.Append($" [{item.FileExt}: {item.Count}]");
            }
            int fileCount = fileExtCounts.Sum(x => x.Count);
            return sb.ToString();

        }

        /// <summary>
        /// 对照片和视频文件生成Preview Name
        /// </summary>
        /// <param name="fileInfoPre"></param>
        public static void PreviewNameMediaFileWithDate(FileInfoPreview fileInfoPre)
        {
            List<string>? videoExts = ConfigHelper.ReadKeyValueIntoList("VideoExts");
            List<string>? photoExts = ConfigHelper.ReadKeyValueIntoList("PhotoExts");
            ShellObject shell = ShellObject.FromParsingName(fileInfoPre.FileInfo.FullName);
            if (videoExts != null && videoExts.Contains(fileInfoPre.FileInfo.Extension.Substring(1).ToUpper()))
            {
                DateTime? videoMediaCreated = shell.Properties.System.Media.DateEncoded.Value;
                if (videoMediaCreated != null)
                {
                    fileInfoPre.NamePreview = videoMediaCreated.Value.ToString("yyyy-MM-dd") + " " + fileInfoPre.FileInfo.Name;
                }
            }
            else if (photoExts != null && photoExts.Contains(fileInfoPre.FileInfo.Extension.Substring(1).ToUpper()))
            {
                DateTime? photoDateTaken = shell.Properties.System.Photo.DateTaken.Value;
                if (photoDateTaken != null)
                {
                    fileInfoPre.NamePreview = photoDateTaken.Value.ToString("yyyy-MM-dd") + " " + fileInfoPre.FileInfo.Name;
                }
            }
        }

        /// <summary>
        /// 检查一个文件的扩展名称是否是指定的照片或者视频格式
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public static bool IsFilePhotoOrVideo(FileInfo fileInfo)
        {
            List<string>? videoExts = ConfigHelper.ReadKeyValueIntoList("VideoExts");
            List<string>? photoExts = ConfigHelper.ReadKeyValueIntoList("PhotoExts");
            List<string>? exts = new List<string>(videoExts);
            exts.AddRange(photoExts);
            if (!string.IsNullOrEmpty(fileInfo.Extension))
            {
                return exts.Contains(fileInfo.Extension.Substring(1).ToUpper());
            }
            else
            {
                return false;
            }
        }
        //public static IEnumerable<FileInfoPreview> FilterPhotoVideoOnly(IEnumerable<FileInfoPreview> fileInfoPres)
        //{
        //    List<string>? videoExts = ConfigHelper.ReadKeyValueIntoList("VideoExts");
        //    List<string>? photoExts = ConfigHelper.ReadKeyValueIntoList("PhotoExts");
        //    List<string>? exts = new List<string>(videoExts);
        //    exts.AddRange(photoExts);
        //    return fileInfoPres.Where(x => exts.Contains(x.FileInfo.Extension.Substring(1).ToUpper()));
        //}

        /// <summary>
        /// 重命名文件
        /// </summary>
        /// <param name="fileInfoPre"></param>
        public static void RenameFile(FileInfoPreview fileInfoPre)
        {
            string newPath = Path.Combine(fileInfoPre.FileInfo.DirectoryName!, fileInfoPre.NamePreview);
            fileInfoPre.FileInfo.MoveTo(newPath);
        }
    }
}
