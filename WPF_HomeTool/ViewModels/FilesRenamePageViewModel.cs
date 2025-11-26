using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using WPF_HomeTool.Helpers;
using WPF_HomeTool.Models;
using System.Text.RegularExpressions;

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
        private bool _IsModifyExtensionPanelVisiable = false;
        [ObservableProperty]
        private bool _IsReplaceNamePanelVisiable = false;
        [ObservableProperty]
        private string _AppendText;
        [ObservableProperty]
        private int _AppendPositionIndex;
        [ObservableProperty]
        private string _NewExtensionText;
        [ObservableProperty]
        private string _SearchText;
        [ObservableProperty]
        private string _ReplaceText;


        private string _appendPosition;
        private string _repalcePosition;
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
                IsModifyExtensionPanelVisiable = false;
                IsReplaceNamePanelVisiable = false;
            }
            else if (SelectedMode == "修改文件扩展名")
            {
                IsModifyExtensionPanelVisiable = true;
                IsAppendTextPanelVisiable = false;
                IsReplaceNamePanelVisiable = false;
            }
            else if (SelectedMode == "删除和替换文字")
            {
                IsReplaceNamePanelVisiable = true;
                IsAppendTextPanelVisiable = false;
                IsModifyExtensionPanelVisiable = false;
            }
            else
            {
                IsAppendTextPanelVisiable = false;
                IsModifyExtensionPanelVisiable = false;
                IsReplaceNamePanelVisiable = false;
            }
        }

        [RelayCommand]
        private void RemoveFiles(DataGrid grid)
        {
            //因为在DataGrid中设置了SelectionUnit="Cell"，所以无法直接获取SelectedItems（一直为空）。所以使用另一种方法，来获取选中的行。
            if (grid == null) return;
            var selectedRows = grid.SelectedCells
                       .Select(c => c.Item)
                       .OfType<FileInfoPreview>()
                       .Distinct()
                       .ToList();
            //IList objectList = selectedItems as IList;
            //List<FileInfoPreview> selectedDirectoryInfos = objectList.Cast<FileInfoPreview>().ToList();
            //foreach (FileInfoPreview fileInfoPre in selectedDirectoryInfos)
            foreach (FileInfoPreview fileInfoPre in selectedRows)
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
            ToastNotificationHelper.ShowSimpleToast("title","message");
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
                foreach (FileInfoPreview fileInfoPreview in Files)
                {
                    if (SelectedMode == "照片视频添加日期")
                    {
                        FileHelper.PreviewNameMediaFileWithDate(fileInfoPreview);
                    }
                    else if (SelectedMode == "指定位置插入文字")
                    {
                        appendNamePreview(fileInfoPreview);
                    }
                    else if (SelectedMode == "修改文件扩展名")
                    {
                        renameExtension(fileInfoPreview);
                    }
                    else if (SelectedMode == "删除和替换文字")
                    {
                        ReplaceFileNamePreview(fileInfoPreview);
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
            _appendPosition = tag;
        }
        [RelayCommand]
        private void ReplaceRadioButtonSelected(string tag)
        {
            _repalcePosition = tag;
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
            string name = string.IsNullOrEmpty(fileInfoPreview.NamePreview) ? fileInfoPreview.FileInfo.Name : fileInfoPreview.NamePreview;
            string replacedText = ReplaceSpecialString(fileInfoPreview, AppendText);
            fileInfoPreview.NamePreview = appendTextIntoName(name, replacedText);
        }

        private string appendTextIntoName(string name, string text)
        {
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(text))
            {
                if (_appendPosition == "Head")
                {
                    return text + name;
                }
                else if (_appendPosition == "End")
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
            if (!string.IsNullOrEmpty(text))
            {
                string folderName = fileInfoPreview.FileInfo.Directory.Name;
                text = text.Replace("$F$", folderName);
                int? index = fileInfoPreview.indexInFolder ?? fileInfoPreview.indexInApp;
                if (index != null)
                {
                    string pattern = @"\$0*I\$";
                    var match = Regex.Match(text, pattern);
                    if (match.Success)
                    {
                        int digital = match.Value.Count(c => c == '0')+1;
                        string fomateParameter = "D" + digital;
                        text = Regex.Replace(text, pattern, index?.ToString(fomateParameter));
                    }
                }
            }
            return text;
        }

        private void renameExtension(FileInfoPreview fileInfoPreview)
        {
            string name = string.IsNullOrEmpty(fileInfoPreview.NamePreview) ? fileInfoPreview.FileInfo.Name : fileInfoPreview.NamePreview;
            //新扩展名是否包含“.”都不影响重命名
            fileInfoPreview.NamePreview = Path.ChangeExtension(name, NewExtensionText);
        }

        private void ReplaceFileNamePreview(FileInfoPreview fileInfoPreview)
        {
            if (_repalcePosition == "All")
            {
                string replacedText = ReplaceSpecialString(fileInfoPreview, ReplaceText);
                string replacedTextWithExt = replacedText + fileInfoPreview.FileInfo.Extension;
                fileInfoPreview.NamePreview = replacedTextWithExt;
            }
            else if (_repalcePosition == "MatchText")
            {
                string name = string.IsNullOrEmpty(fileInfoPreview.NamePreview) ? fileInfoPreview.FileInfo.Name : fileInfoPreview.NamePreview;
                string replacedText = ReplaceSpecialString(fileInfoPreview, ReplaceText);
                string newName = name.Replace(SearchText, replacedText);
                fileInfoPreview.NamePreview = newName;
            }
            //string name = string.IsNullOrEmpty(fileInfoPreview.NamePreview) ? fileInfoPreview.FileInfo.Name : fileInfoPreview.NamePreview;
        }
    }
}
