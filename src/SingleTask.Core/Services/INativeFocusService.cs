using System.Threading.Tasks;

namespace SingleTask.Core.Services
{
    public interface INativeFocusService
    {
        void StartSession(string title, string content);
        void StopSession();
        Task PlaySuccessSoundAsync();
        void TriggerHapticFeedback();
        Task<bool> CheckNotificationPermissionAsync();
        Task RequestNotificationPermissionAsync();
    }
}
