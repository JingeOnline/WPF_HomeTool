using CsvHelper.Configuration.Attributes;
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
        [property: Ignore]
        [ObservableProperty]
        private string _imageUrl;
        [property: Ignore]
        [ObservableProperty]
        private string _albumName;
        [property: Ignore]
        [ObservableProperty]
        private string _albumUrl;
        [ObservableProperty]
        private int _indexInAlbum;
        [property: Ignore]
        [ObservableProperty]
        private WebImageDownloadStatus _downloadStatus=WebImageDownloadStatus.UnDownload;
        //[ObservableProperty]
        //private bool _isSkipped;
        [property: Ignore]
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
