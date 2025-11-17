using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_HomeTool.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
            {
                return Visibility.Collapsed;
            }

            bool boolValue = (bool)value;

            // Invert the logic if the "Invert" string parameter is passed.
            // 反向示例：
            // Visibility="{Binding ElementName=VisibilityToggle, Path=IsChecked, Converter={StaticResource BoolToVisibilityConverter}, ConverterParameter=Invert}"
            if (parameter != null && parameter.ToString().Equals("Invert", StringComparison.OrdinalIgnoreCase))
            {
                boolValue = !boolValue;
            }

            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Visibility))
            {
                return false;
            }

            bool isVisible = (Visibility)value == Visibility.Visible;

            // Invert the logic if the "Invert" string parameter is passed.
            if (parameter != null && parameter.ToString().Equals("Invert", StringComparison.OrdinalIgnoreCase))
            {
                isVisible = !isVisible;
            }

            return isVisible;
        }
    }
}
