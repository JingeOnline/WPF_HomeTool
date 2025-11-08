using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_HomeTool.Controls;

namespace WPF_HomeTool.Helpers
{
    public class FileHelper
    {

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

        public static List<FileInfo> GetAllFilesRecursively(DirectoryInfo dirInfo)
        {
            try
            {
                // Use SearchOption.AllDirectories to include all subdirectories
                return dirInfo.GetFiles("*.*", SearchOption.AllDirectories).ToList();
            }
            catch (UnauthorizedAccessException ex)
            {
                ModernMessageBox.Show(dirInfo.FullName+Environment.NewLine+ex.Message,
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
    }
}
