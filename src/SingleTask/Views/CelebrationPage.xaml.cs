using SingleTask.Core.ViewModels;

namespace SingleTask.Views;

public partial class CelebrationPage : ContentPage
{
    public CelebrationPage(CelebrationViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // FR-001: Wrap async void event handler in try-catch
        try
        {
            await TriggerCelebration();
        }
        catch (Exception ex)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[CelebrationPage.OnAppearing] Error: {ex.Message}");
#endif
        }
    }

    private async Task TriggerCelebration()
    {
        // Simple entrance animation
        await this.Content.ScaleTo(0.9, 100);
        await this.Content.ScaleTo(1.0, 200, Easing.SpringOut);

        // Haptic - FR-003: Add debug logging to catch block
        try
        {
            HapticFeedback.Perform(HapticFeedbackType.LongPress);
        }
        catch (Exception ex)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[CelebrationPage.TriggerCelebration] Haptic error: {ex.Message}");
#endif
        }
    }
}
