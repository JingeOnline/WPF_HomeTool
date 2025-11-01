using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_HomeTool.Navigation
{
    public interface INavigationService
    {
        void Navigate(Type type);
        void SetFrame(Frame frame);
    }
}
