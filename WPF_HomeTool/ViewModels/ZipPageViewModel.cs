using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WPF_HomeTool.Helpers;
using WPF_HomeTool.Models;

namespace WPF_HomeTool.ViewModels
{
    public partial class ZipPageViewModel : ObservableObject, IFileDragDropTarget
    {
        [ObservableProperty]
        private ObservableCollection<FolderZipModel> _FolderZipModelCollection=new ObservableCollection<FolderZipModel>();




        [RelayCommand]
        private void StartZip()
        {
            foreach (var folderZipModel in FolderZipModelCollection)
            {
                FileHelper.ZipFolderWithProgressAsync(folderZipModel,System.IO.Compression.CompressionLevel.NoCompression);
            }
        }

        private void SetZipFilePathToOrigional(FolderZipModel folderZipModel)
        {
            string parentFolder = folderZipModel.FolderDirectoryInfo.Parent.FullName;
            string zipFilePath = Path.Combine(parentFolder, folderZipModel.FolderName + ".zip");
            folderZipModel.ZipFilePath = zipFilePath;
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
                    SetZipFilePathToOrigional (folderZipModel);
                    FolderZipModelCollection.Add(folderZipModel);
                }
            }
        }

    }
}

