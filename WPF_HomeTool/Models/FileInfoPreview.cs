using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_HomeTool.Helpers;

namespace WPF_HomeTool.Models
{
    public class FileInfoPreview
    {
        public FileInfo FileInfo { get; set; }
        public string NamePreview { get; set; }
        public string SizeReadable {  get; set; }
        public FileInfoPreview(FileInfo fileInfo)
        {
            this.FileInfo = fileInfo;
            this.SizeReadable = FileHelper.ConvertBytesToHumanReadable(fileInfo.Length);
        }

    }
}
