using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_HomeTool.Models;

namespace WPF_HomeTool.Converters
{
    public class WebImageDownloadStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is WebImageDownloadStatus status)
            {
                switch (status)
                {
                    case WebImageDownloadStatus.UnDownload:
                        return new SolidColorBrush(Colors.Transparent);

                    case WebImageDownloadStatus.Downloading:
                        return new SolidColorBrush(Colors.LightBlue);

                    case WebImageDownloadStatus.Downloaded:
                        return new SolidColorBrush(Colors.LightGreen);

                    case WebImageDownloadStatus.Failed:
                        return new SolidColorBrush(Colors.LightCoral);

                    case WebImageDownloadStatus.Skipped:
                        return new SolidColorBrush(Colors.LightGray);

                    default:
                        return new SolidColorBrush(Colors.Transparent);
                }
            }

            return new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
