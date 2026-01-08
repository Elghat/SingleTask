using Android.Media;
using SingleTask.Core.Services;

namespace SingleTask.Platforms.Android.Services;

public class AudioService : IAudioService
{
    public Task PlaySuccessSoundAsync()
    {
        return Task.Run(() =>
        {
            try
            {
                var toneGen = new ToneGenerator(global::Android.Media.Stream.Music, 100);
                toneGen.StartTone(Tone.PropBeep, 150);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Audio Error: {ex.Message}");
            }
        });
    }

    public Task PlayCelebrationSoundAsync()
    {
        return Task.Run(() =>
        {
            try
            {
                // Try to play 'trumpet' from raw resources if it existed, 
                // but since we don't have the file yet, we will use a sequence of tones for now.
                // Later user can drop 'trumpet.mp3' in Resources/Raw and we can update this.

                var toneGen = new ToneGenerator(global::Android.Media.Stream.Music, 100);
                toneGen.StartTone(Tone.Dtmf1, 200);
                Thread.Sleep(250);
                toneGen.StartTone(Tone.Dtmf3, 200);
                Thread.Sleep(250);
                toneGen.StartTone(Tone.Dtmf5, 600);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Audio Error: {ex.Message}");
            }
        });
    }
}
