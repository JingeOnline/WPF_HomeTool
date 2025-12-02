using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_HomeTool.Helpers;

namespace WPF_HomeTool.Models
{
    public partial class FolderZipModel:ObservableObject
    {
        [ObservableProperty]
        private string _FolderName;
        [ObservableProperty]
        private int _FolderFilesCount;
        //[ObservableProperty]
        //private string _FolderFilesExtCountString;
        [ObservableProperty]
        private string _FolderRawSizeReadable;
        [ObservableProperty]
        private string _FolderZipSizeReadable;
        [ObservableProperty]
        private string _ZipFilePath;
        [ObservableProperty]
        private double _Progress;

        public DirectoryInfo FolderDirectoryInfo { get; set; }
        public FileInfo ZipFileInfo {  get; set; }

        public FolderZipModel(DirectoryInfo directoryInfo)
        {
            FolderDirectoryInfo = directoryInfo;
            FolderName=FolderDirectoryInfo.Name;
            InitialAsync();
        }

        private async Task InitialAsync()
        {
            FolderRawSizeReadable=FileHelper.GetDirectorySizeReadable(FolderDirectoryInfo);
            FolderFilesCount = FileHelper.GetDirectoryFilesCount(FolderDirectoryInfo);
        }
    }
}
