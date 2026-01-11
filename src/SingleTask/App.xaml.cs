using SingleTask.Core.Services;

namespace SingleTask;

public partial class App : Application
{
    private readonly IDatabaseService _databaseService;

    public App(IDatabaseService databaseService)
    {
        InitializeComponent();
        _databaseService = databaseService;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }

    /// <summary>
    /// Called when the app goes to background.
    /// Closes the database to ensure all WAL writes are flushed to disk.
    /// This prevents data loss when Android force-kills the app.
    /// </summary>
    protected override void OnSleep()
    {
        base.OnSleep();

        // Fire-and-forget with proper exception handling
        _ = CloseDatabaseAsync();
    }

    private async Task CloseDatabaseAsync()
    {
        try
        {
            await _databaseService.CloseAsync();
        }
        catch (Exception ex)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[App.OnSleep] Error closing database: {ex.Message}");
#endif
        }
    }
}