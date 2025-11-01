namespace WPF_HomeTool.Navigation;

/// <summary>
/// Service for navigating between pages.
/// </summary>
public class NavigationService : INavigationService
{
    private Frame _frame;
    private readonly IServiceProvider _serviceProvider;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void SetFrame(Frame frame)
    {
        _frame = frame;
    }
    //使用容器，获取对应类型的页面实例，然后导航到该页面
    public void Navigate(Type type)
    {
        if (type != null)
        {
            var page = _serviceProvider.GetRequiredService(type);
            _frame.Navigate(page);
        }
    }

}
