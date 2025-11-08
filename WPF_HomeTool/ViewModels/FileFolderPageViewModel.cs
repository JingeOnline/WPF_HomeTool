using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_HomeTool.Helpers;
using WPF_HomeTool.Models;

namespace WPF_HomeTool.ViewModels
{
    public partial class FileFolderPageViewModel : ObservableObject, IFileDragDropTarget
    {
        [ObservableProperty]
        private ObservableCollection<FileInfoPreview> _Files = new ObservableCollection<FileInfoPreview>();
        [ObservableProperty]
        private int _FilesCount;

        public FileFolderPageViewModel()
        {

        }

        public void OnFileDrop(string[] paths)
        {
            foreach (string filePath in paths)
            {
                FileInfo fileInfo = new FileInfo(filePath);
                if (fileInfo.Exists)
                {
                    Files.Add(new FileInfoPreview(fileInfo));
                }
                else
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(filePath);
                    if (dirInfo.Exists)
                    {
                        var FileInfoList = FileHelper.GetAllFilesRecursively(dirInfo);
                        foreach (var fileInFolder in FileInfoList)
                        {
                            Files.Add(new FileInfoPreview(fileInFolder));
                        }
                    }
                }
            }
            FilesCount = Files.Count;
        }
    }
}
