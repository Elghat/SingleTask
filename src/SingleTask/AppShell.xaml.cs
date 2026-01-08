using SingleTask.Views;

namespace SingleTask;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(FocusPage), typeof(FocusPage));
        Routing.RegisterRoute(nameof(CelebrationPage), typeof(CelebrationPage));
    }
}