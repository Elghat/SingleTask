namespace SingleTask.Core.Services;

public interface IAudioService
{
    Task PlaySuccessSoundAsync();
    Task PlayCelebrationSoundAsync();
}
