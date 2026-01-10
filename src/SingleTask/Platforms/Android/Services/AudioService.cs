using Android.Media;
using SingleTask.Core.Services;

namespace SingleTask.Platforms.Android.Services;

public class AudioService : IAudioService
{
    public async Task PlaySuccessSoundAsync()
    {
        try
        {
            var toneGen = new ToneGenerator(global::Android.Media.Stream.Music, 100);
            toneGen.StartTone(Tone.PropBeep, 150);
            await Task.Delay(200); // Allow tone to play
            toneGen.Release(); // FR-015: Proper disposal
        }
        catch (Exception ex)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[AudioService.PlaySuccessSoundAsync] Error: {ex.Message}");
#endif
        }
    }

    /// <summary>
    /// FR-002: Replaced Thread.Sleep with Task.Delay for non-blocking async.
    /// FR-015: Added ToneGenerator.Release() for proper resource disposal.
    /// </summary>
    public async Task PlayCelebrationSoundAsync()
    {
        try
        {
            var toneGen = new ToneGenerator(global::Android.Media.Stream.Music, 100);

            toneGen.StartTone(Tone.Dtmf1, 200);
            await Task.Delay(250); // FR-002: Non-blocking delay
            toneGen.StartTone(Tone.Dtmf3, 200);
            await Task.Delay(250); // FR-002: Non-blocking delay
            toneGen.StartTone(Tone.Dtmf5, 600);
            await Task.Delay(650); // Allow final tone to complete

            toneGen.Release(); // FR-015: Proper disposal
        }
        catch (Exception ex)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[AudioService.PlayCelebrationSoundAsync] Error: {ex.Message}");
#endif
        }
    }
}
