namespace WPF_HomeTool.ViewModels;

/// <summary>
/// Converts an image id to a brush
/// </summary>
internal sealed class ImageIdToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string imageKey = (string)value;
        return Application.Current.Resources[imageKey];
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
