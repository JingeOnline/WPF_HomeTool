using Microsoft.Web.WebView2.WinForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_HomeTool.Models
{
    public class WebPageTabModel
    {
        //public string Name { get; set; }
        //public WebView2 WebView { get; set; }
        //public WebPageTabModel(string name,Uri uri)
        //{
        //    Name = name;
        //    WebView = new WebView2();
        //    WebView.Source = uri;
        //}
        public string Name { get; set; }
        public Uri Source { get; set; }

        public WebPageTabModel(string name, Uri uri)
        {
            Name = name;
            Source = uri;
        }
    }
}
