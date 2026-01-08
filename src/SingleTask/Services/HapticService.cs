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
        catch
        {
            // Ignore if device doesn't support it
        }
    }

    public void VibrateSuccess()
    {
        try
        {
            // Vibrate for 500ms for success
            Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(500));
        }
        catch
        {
            // Ignore
        }
    }
}
