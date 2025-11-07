using WPF_HomeTool.Helpers;
using WPF_HomeTool.Navigation;
using WPF_HomeTool.ViewModels;
using WPF_HomeTool.Views;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using System.Runtime.CompilerServices;

namespace WPF_HomeTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //这里定义ViewModel属性，在XAML中绑定时，绑定VM的所有属性，都要加上ViewModel.前缀
        public MainWindowViewModel VM { get; }
        private readonly INavigationService _navigationService;
        public Type CurrentPageType;

        public MainWindow(MainWindowViewModel viewModel, IServiceProvider serviceProvider, INavigationService navigationService)
        {
            VM = viewModel;
            _navigationService = navigationService;

            DataContext = this;
            InitializeComponent();

            //设置导航框架(该代码必须写在InitializeComponent方法后面，否则找不到对应的控件)
            _navigationService.SetFrame(this.RootContentFrame);
            //默认导航到首页
            _navigationService.Navigate(typeof(HomePage));


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
        }

        //导航完成后，更新BackButton的可用性，并更新导航列表的选择
        private void RootContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            BackButton.IsEnabled = RootContentFrame.CanGoBack;

            //在导航后，更新导航列表的选择
            Type type = e.Content!.GetType()!;
            if (CurrentPageType != type)
            {
                CurrentPageType = type;
                foreach (ListBoxItem item in NavigationListBox.Items)
                {
                    string? tag = (string?)item.Tag;
                    if (tag != null)
                    {
                        Type pageType = Type.GetType(tag)!;
                        if (pageType == type)
                        {
                            NavigationListBox.SelectedItem = item;
                            break;
                        }
                    }
                }
            }
            else
            {
                return;
            }

        }

        //导航列表选择变化时，导航到对应的页面
        private void NavigationListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem? selectedItem = (ListBoxItem?)NavigationListBox.SelectedItem;
            if (selectedItem != null)
            {
                string? tag = (string?)selectedItem.Tag;
                if (tag != null && tag != CurrentPageType.FullName)
                {
                    //把文字转换为Type类型，然后导航到对应的页面。后面的叹号是告诉编译器，这个值不为null。
                    Type pageType = Type.GetType(tag)!;
                    _navigationService.Navigate(pageType);
                }
            }
        }


        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationListBox.SelectedItem = null;
            _navigationService.Navigate(typeof(SettingsPage));
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (RootContentFrame.CanGoBack)
            {
                RootContentFrame.GoBack();
            }
        }

        private void TrayIconShowWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Show();
            WindowState = WindowState.Normal;
        }

        private void TrayIconExit_Click(object sender, RoutedEventArgs e)
        {
            TrayIcon.Dispose();
            Application.Current.Shutdown();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;//取消默认的按下关闭按钮的行为，否则会触发程序的关闭
            this.Hide();
            TrayIcon.Visibility = Visibility.Visible;//显示托盘
        }
    }
}