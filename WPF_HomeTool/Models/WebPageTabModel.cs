using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_HomeTool.Models
{
    public partial class WebPageTabModel:ObservableObject
    {
        [ObservableProperty]
        private string _Name;
        [ObservableProperty]
        private WebView2 _WebView;
        public WebPageTabModel(string name, Uri uri)
        {
            Name = name;
            WebView = new WebView2();
            WebView.Source = uri;
            Debug.WriteLine("Initial "+uri);
            //InitializeWebView(uri);
        }
        //private async void InitializeWebView(Uri uri)
        //{
        //    await WebView.EnsureCoreWebView2Async();
        //    WebView.Source = uri;
        //}

    }
}
