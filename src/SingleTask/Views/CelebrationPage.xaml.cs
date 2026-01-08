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
        // Trigger simple confetti/celebration animation
        await TriggerCelebration();
    }

    private async Task TriggerCelebration()
    {
        // Simple entrance animation
        await this.Content.ScaleTo(0.9, 100);
        await this.Content.ScaleTo(1.0, 200, Easing.SpringOut);

        // Haptic
        try { HapticFeedback.Perform(HapticFeedbackType.LongPress); } catch { }
    }
}
