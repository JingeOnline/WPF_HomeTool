using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_HomeTool.Converters
{
    public class StringEqualityConverter : IValueConverter
    {
        // Convert (Value is the SelectedOption from the ViewModel, TargetType is boolean)
        // Determines if the RadioButton should be checked.
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 'parameter' is the string of the current RadioButton's item.
            // 'value' is the ViewModel's SelectedOption string.
            return value != null && value.Equals(parameter);
        }

        // ConvertBack (Value is the boolean IsChecked state, TargetType is string)
        // Sets the SelectedOption property in the ViewModel.
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // If the RadioButton is checked (value == true), return the current item's string (parameter).
            if (value is bool isChecked && isChecked)
            {
                return parameter;
            }
            return Binding.DoNothing;
        }
    }
}
