using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_HomeTool.Helpers;

namespace WPF_HomeTool.Models
{
    public partial class FileInfoPreview : ObservableObject
    {

        private bool isRecordInHistory = true;
        private Stack<string> NamePreviewHistory { get; set; } = new Stack<string>();
        public int? indexInFolder;
        public int? indexInApp;

        [ObservableProperty]
        private FileInfo _FileInfo;
        [ObservableProperty]
        private string _NamePreview=string.Empty;
        partial void OnNamePreviewChanged(string? oldValue, string newValue)
        {
            if (oldValue != newValue && isRecordInHistory)
            {
                NamePreviewHistory.Push(oldValue);
            }
        }
        [ObservableProperty]
        private string _SizeReadable;

        public FileInfoPreview(FileInfo fileInfo, int? indexInFolder=null, int? indexInApp=null)
        {
            this.FileInfo = fileInfo;
            this.SizeReadable = FileHelper.ConvertBytesToHumanReadable(fileInfo.Length);
            this.indexInFolder = indexInFolder;
            this.indexInApp = indexInApp;
        }

        public void ReversePreview()
        {
            if (NamePreviewHistory.Count > 0)
            {
                isRecordInHistory = false;
                NamePreview = NamePreviewHistory.Pop();
                isRecordInHistory = true;
            }
        }
    }
}
