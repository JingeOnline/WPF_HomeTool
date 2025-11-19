using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Shell;

namespace WPF_HomeTool.Controls
{
    /// <summary>
    /// Interaction logic for ModernMessageBox.xaml
    /// </summary>
    public partial class ModernMessageBox : Window
    {
        public MessageBoxResult Result { get; private set; }

        private ModernMessageBox(string message, string title, MessageBoxButton buttons, MessageBoxImage icon)
        {
            InitializeComponent();

            //这里设置窗口样式，让窗口有圆角和阴影
            WindowChrome.SetWindowChrome(
                this,
                new WindowChrome
                {
                    CaptionHeight = 48, // 标题栏可拖动区域的高度
                    CornerRadius = new CornerRadius(12),
                    GlassFrameThickness = new Thickness(-1),
                    ResizeBorderThickness = ResizeMode == ResizeMode.NoResize ? default : new Thickness(4),
                    UseAeroCaptionButtons = true,
                    NonClientFrameEdges = SystemParameters.HighContrast ? NonClientFrameEdges.None :
                        NonClientFrameEdges.Right | NonClientFrameEdges.Bottom | NonClientFrameEdges.Left
                }
            );

            TitleText.Text = title;
            MessageText.Text = message;

            SetIcon(icon);
            SetButtons(buttons);
        }

        private void SetIcon(MessageBoxImage icon)
        {
            IconText.Visibility = Visibility.Visible;

            switch (icon)
            {
                case MessageBoxImage.Information:
                    IconText.Text = "\uE946"; // Info icon
                    IconText.Foreground = new System.Windows.Media.SolidColorBrush(
                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#0078D4"));
                    break;
                case MessageBoxImage.Warning:
                    IconText.Text = "\uE7BA"; // Warning icon
                    IconText.Foreground = new System.Windows.Media.SolidColorBrush(
                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FFA500"));
                    break;
                case MessageBoxImage.Error:
                    IconText.Text = "\uE783"; // Error icon
                    IconText.Foreground = new System.Windows.Media.SolidColorBrush(
                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#D13438"));
                    break;
                case MessageBoxImage.Question:
                    IconText.Text = "\uE9CE"; // Question icon
                    IconText.Foreground = new System.Windows.Media.SolidColorBrush(
                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#0078D4"));
                    break;
                default:
                    IconText.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void SetButtons(MessageBoxButton buttons)
        {
            ButtonPanel.Children.Clear();

            switch (buttons)
            {
                case MessageBoxButton.OK:
                    AddButton("OK", MessageBoxResult.OK, true);
                    break;
                case MessageBoxButton.OKCancel:
                    AddButton("Cancel", MessageBoxResult.Cancel, false);
                    AddButton("OK", MessageBoxResult.OK, true);
                    break;
                case MessageBoxButton.YesNo:
                    AddButton("No", MessageBoxResult.No, false);
                    AddButton("Yes", MessageBoxResult.Yes, true);
                    break;
                case MessageBoxButton.YesNoCancel:
                    AddButton("Cancel", MessageBoxResult.Cancel, false);
                    AddButton("No", MessageBoxResult.No, false);
                    AddButton("Yes", MessageBoxResult.Yes, true);
                    break;
            }
        }

        private void AddButton(string content, MessageBoxResult result, bool isPrimary)
        {
            var button = new System.Windows.Controls.Button
            {
                Content = content,
                Width=60,
                Margin = new Thickness(8, 0, 0, 0),
                Style = isPrimary ?
                    //(Style)FindResource("ModernButtonStyle") :
                    //WPF新的按钮样式自带AccentButtonStyle
                    (Style)FindResource("AccentButtonStyle") :
                    (Style)FindResource("SecondaryButtonStyle")
            };

            button.Click += (s, e) =>
            {
                Result = result;
                DialogResult = true;
                Close();
            };
            if (isPrimary) { button.IsDefault = true; }
            ButtonPanel.Children.Add(button);
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Cancel;
            Close();
        }

        public static MessageBoxResult Show(string message, string title = "Message",
            MessageBoxButton buttons = MessageBoxButton.OK,
            MessageBoxImage icon = MessageBoxImage.None)
        {
            var msgBox = new ModernMessageBox(message, title, buttons, icon);
            msgBox.ShowDialog();
            return msgBox.Result;
        }
    }

    public enum MessageBoxButton
    {
        OK,
        OKCancel,
        YesNo,
        YesNoCancel
    }

    public enum MessageBoxImage
    {
        None,
        Information,
        Warning,
        Error,
        Question
    }

    public enum MessageBoxResult
    {
        None = 0,
        OK = 1,
        Cancel = 2,
        Yes = 6,
        No = 7
    }
}
