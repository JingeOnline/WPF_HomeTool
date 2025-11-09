using System;
using System.Collections;
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
        private string _SelectedMode;
        [ObservableProperty]
        private ObservableCollection<FileInfoPreview> _Files = new ObservableCollection<FileInfoPreview>();
        [ObservableProperty]
        private string _FilesCount;
        [ObservableProperty]
        private string _Hint;

        public FileFolderPageViewModel()
        {

        }

        [RelayCommand]
        private void RemoveFiles(object selectedItems)
        {
            IList objectList = selectedItems as IList;
            List<FileInfoPreview> selectedDirectoryInfos = objectList.Cast<FileInfoPreview>().ToList();

            foreach (FileInfoPreview fileInfoPre in selectedDirectoryInfos)
            {
                Files.Remove(fileInfoPre);
            }
            FilesCount = FileHelper.GetDirectoryFileExtCountString(Files.Select(x => x.FileInfo));
        }
        [RelayCommand]
        private void AddFiles()
        {
            //官方教程 https://learn.microsoft.com/en-us/dotnet/desktop/wpf/windows/how-to-open-common-system-dialog-box
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Multiselect = true;
            bool? result = dialog.ShowDialog();
            if(result==true)
            {
                OnFileDrop(dialog.FileNames);
            }
            FilesCount = FileHelper.GetDirectoryFileExtCountString(Files.Select(x => x.FileInfo));
        }
        [RelayCommand]
        private void ClearFiles()
        {
            _Files.Clear();
            FilesCount = FileHelper.GetDirectoryFileExtCountString(Files.Select(x => x.FileInfo));
        }
        [RelayCommand]
        private void Preview()
        {
            foreach (var item in Files)
            {
                FileHelper.RenameMediaFileWithDate(item);
            }
        }

        //响应页面上拖拽过来的文件或者文件夹
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
            FilesCount = FileHelper.GetDirectoryFileExtCountString(Files.Select(x=>x.FileInfo));
            if(SelectedMode== "照片视频添加日期")
            {
                //TODO:筛选掉非媒体格式
            }
        }
    }
}
