using SingleTask.Core.Services;

namespace SingleTask.Services;

public class HapticService : IHapticService
{
    public void VibrateShort()
    {
        try
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
        }
        catch (Exception ex)
        {
            // Ignore if device doesn't support it
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[HapticService.VibrateShort] Error: {ex.Message}");
#endif
        }
    }

    public void VibrateSuccess()
    {
        try
        {
            // Vibrate for 500ms for success
            Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(500));
        }
        catch (Exception ex)
        {
            // Ignore
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[HapticService.VibrateSuccess] Error: {ex.Message}");
#endif
        }
    }
}
