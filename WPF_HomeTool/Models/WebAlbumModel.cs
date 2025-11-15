using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_HomeTool.Models
{
    public partial class WebAlbumModel: ObservableObject
    {
        [ObservableProperty]
        private string _albumName;
        [ObservableProperty]
        private string _albumUrl;
        [ObservableProperty]
        private int _totalImageCount;
        //[ObservableProperty]
        //private int _downloadedImageCount;
        public List<WebImageModel> WebImageModelList { get; set; }= new List<WebImageModel>();
        public WebAlbumModel(string uri)
        {
            AlbumUrl = uri;
        }
    }
}
