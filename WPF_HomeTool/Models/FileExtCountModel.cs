using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_HomeTool.Models
{
    public class FileExtCountModel
    {
        public string FileExt { get; set; }
        public int Count { get; set; }
        public FileExtCountModel(string fileExt, int count)
        {
            this.FileExt = fileExt;
            this.Count = count;
        }
    }
}
