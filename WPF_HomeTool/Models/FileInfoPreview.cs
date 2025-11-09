using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_HomeTool.Helpers;

namespace WPF_HomeTool.Models
{
    public partial class FileInfoPreview: ObservableObject
    {

        [ObservableProperty]
        private FileInfo _FileInfo;
        [ObservableProperty]
        private string _NamePreview;
        [ObservableProperty]
        private string _SizeReadable;
        public FileInfoPreview(FileInfo fileInfo)
        {
            this.FileInfo = fileInfo;
            this.SizeReadable = FileHelper.ConvertBytesToHumanReadable(fileInfo.Length);
        }

    }
}
