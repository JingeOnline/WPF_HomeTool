using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAPICodePack.Shell;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WPF_HomeTool.Controls;
using WPF_HomeTool.Models;
//using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;

namespace WPF_HomeTool.Helpers
{
    public class FileHelper
    {
        private static readonly ILogger<FileHelper> _logger = LoggerFactory.Create(builder => builder.AddNLog()).CreateLogger<FileHelper>();
        private static CsvConfiguration csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            //分隔符
            Delimiter = "\t",
            //是否包含表头
            HasHeaderRecord = false,
            //MissingFieldFound = null,
            //HeaderValidated = null,
        };


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
                _logger.LogError(ex, "递归获取文件夹下的所有文件发生异常");
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
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "对媒体文件提取日期信息时发生异常");
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

        /// <summary>
        /// 重命名文件
        /// </summary>
        /// <param name="fileInfoPre"></param>
        public static void RenameFile(FileInfoPreview fileInfoPre)
        {
            try
            {
                string newPath = Path.Combine(fileInfoPre.FileInfo.DirectoryName!, fileInfoPre.NamePreview);
                fileInfoPre.FileInfo.MoveTo(newPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "重命名文件时发生异常");
            }
        }

        /// <summary>
        /// 替换文件名中不符合Windows规范的字符
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string ReplaceWindowsReservedCharAndTrim(string fileName)
        {
            Char[] unSafeChars = { '*', ':', '\\', '/', '|', '\"', '|', '?', '<', '>' };
            foreach (char c in unSafeChars)
            {
                fileName = fileName.Replace(c, '_');
            }
            return fileName.Trim();
        }

        /// <summary>
        /// 向文件追加一行文本（如果文件不存在会自动创建该文件）
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="line"></param>
        public static void AppendLineToFile(string filePath, string line)
        {
            using (StreamWriter sw = new StreamWriter(filePath, append: true))
            {
                sw.WriteLine(line);
            }
        }

        /// <summary>
        /// 向文本文件中写入多行（如果目标文件不存在，会自动创建）
        /// </summary>
        /// <param name="models"></param>
        public static void AppendLinesToFile(IEnumerable<string> lines, string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
            }

            if (lines == null)
            {
                //throw new ArgumentNullException(nameof(lines));
                return;
            }

            File.AppendAllLines(filePath, lines);
        }

        /// <summary>
        /// 读取文本文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public static IEnumerable<string> ReadFileInLines(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not exist: {filePath}");
            }

            return File.ReadLines(filePath);
        }

        /// <summary>
        /// 将模型转换为CSV格式的单行字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string ConvertModelToCsvLine<T>(T model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            using (var writer = new StringWriter())
            using (var csv = new CsvWriter(writer, csvConfig))
            {
                csv.WriteRecord(model);
                csv.NextRecord();//没有这行代码，输出的字符串为空
                writer.Flush();
                string line = writer.ToString();
                return line.Trim();
            }
        }

        /// <summary>
        /// 将模型列表追加写入CSV文件（如果文件不存在会自动创建该文件）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="models"></param>
        /// <param name="filePath"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void AppendModelsToCsv<T>(List<T> models, string filePath)
        {
            if (models == null || models.Count == 0)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
            }

            using (var stream = File.Open(filePath, FileMode.Append))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer, csvConfig))
            {
                csv.WriteRecords(models);
            }
        }

        /// <summary>
        /// 从CSV文件中读取所有model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public static List<T> ReadCsvFile<T>(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, csvConfig))
            {
                return csv.GetRecords<T>().ToList();
            }
        }

        private static readonly object removeLineFromFileLock = new object();

        /// <summary>
        /// 从文本文件中移除指定行（线程安全）
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="lineToRemove"></param>
        /// <exception cref="FileNotFoundException"></exception>
        public static void RemoveLineFromFile_ThreadSafe(string filePath, string lineToRemove)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            lock (removeLineFromFileLock)
            {
                var lines = File.ReadAllLines(filePath);
                var updatedLines = lines.Where(line => line != lineToRemove).ToList();
                File.WriteAllLines(filePath, updatedLines);
            }
        }

        /// <summary>
        /// 删除文本文件中的所有内容
        /// </summary>
        /// <param name="filePath"></param>
        public static void RemoveAllFromFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                using (var fs = File.Create(filePath)) { }
            }
        }

        public static void CreateFileWithDirectoryIfNotExist(string path)
        {
            // 1. Get the directory part of the path
            string directory = Path.GetDirectoryName(path);

            // 2. Create the directory if it's missing
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // 3. Create the file if it's missing
            if (!File.Exists(path))
            {
                using (File.Create(path)) { }
            }
        }
    }
}
