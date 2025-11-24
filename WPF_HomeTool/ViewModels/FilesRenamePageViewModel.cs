using Microsoft.Extensions.Logging;
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
    public partial class FilesRenamePageViewModel : ObservableObject, IFileDragDropTarget
    {
        private readonly ILogger<FilesRenamePageViewModel> _logger;
        [ObservableProperty]
        private string _SelectedMode;
        [ObservableProperty]
        private ObservableCollection<FileInfoPreview> _Files = new ObservableCollection<FileInfoPreview>();
        [ObservableProperty]
        private string _FilesCount;
        [ObservableProperty]
        private string _Hint;
        [ObservableProperty]
        private bool _IsSaveButtonEnable = false;
        [ObservableProperty]
        private bool _IsPreviewButtonEnable = false;
        [ObservableProperty]
        private bool _IsAppendTextPanelVisiable = false;
        [ObservableProperty]
        private string _AppendText;
        [ObservableProperty]
        private int _AppendPositionIndex;

        private string _appendPosition;
        public FilesRenamePageViewModel(ILogger<FilesRenamePageViewModel> logger)
        {
            _logger = logger;
        }

        [RelayCommand]
        private void SelectedModeChanged()
        {
            if (SelectedMode == "指定位置插入文字")
            {
                IsAppendTextPanelVisiable = true;
            }
            else
            {
                IsAppendTextPanelVisiable = false;
            }
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
            if (result == true)
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
            IsSaveButtonEnable = false;
            IsPreviewButtonEnable = false;
        }
        [RelayCommand]
        private void Preview()
        {
            try
            {
                if (Files == null || Files.Count == 0)
                {
                    return;
                }
                foreach (var item in Files)
                {
                    if (SelectedMode == "照片视频添加日期")
                    {
                        FileHelper.PreviewNameMediaFileWithDate(item);
                    }
                    else if (SelectedMode == "指定位置插入文字")
                    {
                        appendNamePreview(item);
                    }
                }
                IsSaveButtonEnable = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "文件重命名预览出现异常");
            }
        }
        [RelayCommand]
        private void ReversePreview()
        {
            foreach (var item in Files)
            {
                item.ReversePreview();
            }
        }
        [RelayCommand]
        private void Save()
        {
            foreach (var item in Files)
            {
                FileHelper.RenameFile(item);
            }
            Hint = "保存成功";
            IsSaveButtonEnable = false;
        }
        [RelayCommand]
        private void AppendPositionSelected(string tag)
        {
            //Debug.WriteLine(tag);
            _appendPosition = tag;
        }


        //响应页面上拖拽过来的文件或者文件夹
        public void OnFileDrop(string[] paths)
        {
            int indexInApp = 1;
            foreach (string filePath in paths)
            {
                FileInfo fileInfo = new FileInfo(filePath);
                if (fileInfo.Exists)
                {
                    if (SelectedMode == "照片视频添加日期" && FileHelper.IsFilePhotoOrVideo(fileInfo))
                    {
                        Files.Add(new FileInfoPreview(fileInfo, indexInApp: indexInApp));
                    }
                    else if (SelectedMode != "照片视频添加日期")
                    {
                        Files.Add(new FileInfoPreview(fileInfo, indexInApp: indexInApp));
                    }
                    indexInApp++;
                }
                else
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(filePath);
                    if (dirInfo.Exists)
                    {
                        var FileInfoList = FileHelper.GetAllFilesRecursively(dirInfo);
                        int indexInFolder = 1;
                        foreach (var fileInFolder in FileInfoList)
                        {
                            if (SelectedMode == "照片视频添加日期" && FileHelper.IsFilePhotoOrVideo(fileInFolder))
                            {
                                Files.Add(new FileInfoPreview(fileInFolder, indexInFolder: indexInFolder));
                            }
                            else if (SelectedMode != "照片视频添加日期")
                            {
                                Files.Add(new FileInfoPreview(fileInFolder, indexInFolder: indexInFolder));
                            }
                            indexInFolder++;
                        }
                    }
                }
            }
            FilesCount = FileHelper.GetDirectoryFileExtCountString(Files.Select(x => x.FileInfo));
            if (Files.Count > 0)
            {
                IsPreviewButtonEnable = true;
            }
        }

        private void appendNamePreview(FileInfoPreview fileInfoPreview)
        {
            if (!string.IsNullOrEmpty(fileInfoPreview.NamePreview))
            {
                fileInfoPreview.NamePreview = appendTextIntoName(fileInfoPreview.NamePreview, AppendText);
            }
            else
            {
                fileInfoPreview.NamePreview = appendTextIntoName(fileInfoPreview.FileInfo.Name, AppendText);
            }
        }

        private string appendTextIntoName(string name, string text)
        {
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(text))
            {
                if (_appendPosition == "Head")
                {
                    return text + name;
                }
                if (_appendPosition == "End")
                {
                    return Path.GetFileNameWithoutExtension(name) + text + Path.GetExtension(name);
                }
                else
                {
                    return name.Insert(AppendPositionIndex, text);
                }
            }
            else
            {
                return name ?? text;
            }
        }

        private string ReplaceSpecialString(FileInfoPreview fileInfoPreview, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                string folderName = fileInfoPreview.FileInfo.Directory.Name;
                text=text.Replace("$folder$", folderName);
                int? index = fileInfoPreview.indexInFolder ?? fileInfoPreview.indexInApp;
                if(index!=null)
                {
                    text=text.Replace("$index$",index.ToString());
                }
            }
            return text;
        }
    }
}
