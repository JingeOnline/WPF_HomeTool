using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_HomeTool.Models
{
    public partial class WebImageModel:ObservableObject
    {
        [ObservableProperty]
        private string _pageUrl;
        [ObservableProperty]
        private string _imageUrl;
        [ObservableProperty]
        private string _albumName;
        [ObservableProperty]
        private string _albumUrl;
        [ObservableProperty]
        private int _indexInAlbum;
        [ObservableProperty]
        private WebImageDownloadStatus _downloadStatus;
        //[ObservableProperty]
        //private bool _isSkipped;
        [ObservableProperty]
        private TimeSpan _imageDownloadTime;
        [ObservableProperty]
        private string _filePathWithoutExt;
    }

    public enum WebImageDownloadStatus
    {
        UnDownload,
        Downloading,
        Downloaded,
        Failed,
        Skipped
    }
}
