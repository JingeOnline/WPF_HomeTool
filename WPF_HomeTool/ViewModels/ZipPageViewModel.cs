using Microsoft.Extensions.Logging;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WPF_HomeTool.Controls;
using WPF_HomeTool.Helpers;
using WPF_HomeTool.Models;

namespace WPF_HomeTool.ViewModels
{
    public partial class ZipPageViewModel : ObservableObject, IFileDragDropTarget
    {
        private readonly ILogger<ZipPageViewModel> _logger;
        //private Dictionary<string, string> _zipOutputFolderDic;

        [ObservableProperty]
        private ObservableCollection<FolderZipModel> _FolderZipModelCollection = new ObservableCollection<FolderZipModel>();
        [ObservableProperty]
        private FolderZipModel _UserSelectedFolderZipModel;
        [ObservableProperty]
        private bool _IsDelteAfterZip = false;
        [ObservableProperty]
        private bool _IsContainOrigionalFolderInZip = false;
        [ObservableProperty]
        private string _SelectedOutputFolder = "父级文件夹";
        [ObservableProperty]
        private ObservableCollection<string> _OutputFolderCollection = new ObservableCollection<string>();
        [ObservableProperty]
        private ObservableCollection<CompressionLevel> _CompressionLevels;
        [ObservableProperty]
        private CompressionLevel _UserSelectedCompressionLevel = CompressionLevel.NoCompression;

        public ZipPageViewModel(ILogger<ZipPageViewModel> logger)
        {
            _logger = logger;
            OutputFolderCollection.Add("父级文件夹");
            Dictionary<string, string> _zipOutputFolderDic = ConfigHelper.GetConfigSection("zipOutputFolder");
            foreach (KeyValuePair<string, string> pair in _zipOutputFolderDic)
            {
                OutputFolderCollection.Add(pair.Value);
            }
            IEnumerable<CompressionLevel> levels = Enum.GetValues(typeof(CompressionLevel)).Cast<CompressionLevel>();
            CompressionLevels = new ObservableCollection<CompressionLevel>(levels);
        }

        [RelayCommand]
        private void UserSelectOutputDirectory()
        {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
            {
                dialog.IsFolderPicker = true; //Select Folder Only
                dialog.Multiselect = false;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    OutputFolderCollection.Add(dialog.FileName);
                    SelectedOutputFolder = dialog.FileName;
                }
            }
        }
        [RelayCommand]
        private void AddFolders()
        {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
            {
                dialog.IsFolderPicker = true; //Select Folder Only
                dialog.Multiselect = true;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    var folderPathes = dialog.FileNames;
                    foreach (string folderPath in folderPathes)
                    {
                        DirectoryInfo dirInfo = new DirectoryInfo(folderPath);
                        if (dirInfo.Exists)
                        {
                            FolderZipModel folderZipModel = new FolderZipModel(dirInfo);
                            FolderZipModelCollection.Add(folderZipModel);
                        }
                    }
                }
            }
        }

        [RelayCommand]
        private void RemoveFolderZipModel()
        {
            FolderZipModelCollection.Remove(UserSelectedFolderZipModel);
        }
        [RelayCommand]
        private void ClearCollection()
        {
            FolderZipModelCollection.Clear();
        }

        [RelayCommand]
        private async void StartZip()
        {
            try
            {
                foreach (var folderZipModel in FolderZipModelCollection)
                {

                    SetZipFilePath(folderZipModel);
                    await FileHelper.ZipFolderWithProgressAsync(folderZipModel, UserSelectedCompressionLevel, IsContainOrigionalFolderInZip);
                    if (IsDelteAfterZip)
                    {
                        folderZipModel.FolderDirectoryInfo.Delete(recursive: true);
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex,"压缩文件夹时发生异常");
                ModernMessageBox.Show(ex.Message+Environment.NewLine+Environment.NewLine+ex.InnerException?.Message,
                    "压缩文件夹时发生异常",Controls.MessageBoxButton.OK,Controls.MessageBoxImage.Error);
            }
        }

        private void SetZipFilePath(FolderZipModel folderZipModel)
        {
            if (SelectedOutputFolder == "父级文件夹")
            {
                string parentFolder = folderZipModel.FolderDirectoryInfo.Parent.FullName;
                string zipFilePath = Path.Combine(parentFolder, folderZipModel.FolderName + ".zip");
                folderZipModel.ZipFilePath = zipFilePath;
            }
            else
            {
                string zipFilePath = Path.Combine(SelectedOutputFolder, folderZipModel.FolderName + ".zip");
                folderZipModel.ZipFilePath = zipFilePath;
            }
        }

        //响应页面上拖拽过来的文件或者文件夹
        public void OnFileDrop(string[] paths)
        {
            foreach (string filePath in paths)
            {
                DirectoryInfo dirInfo = new DirectoryInfo(filePath);
                if (dirInfo.Exists)
                {
                    FolderZipModel folderZipModel = new FolderZipModel(dirInfo);
                    FolderZipModelCollection.Add(folderZipModel);
                }
            }
        }

    }
}

