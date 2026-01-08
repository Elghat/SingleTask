using System.Threading.Tasks;

namespace SingleTask.Core.Services
{
    public class NativeFocusServiceStub : INativeFocusService
    {
        public Task<bool> CheckNotificationPermissionAsync() => Task.FromResult(true);

        public Task PlaySuccessSoundAsync() => Task.CompletedTask;

        public Task RequestNotificationPermissionAsync() => Task.CompletedTask;

        public void StartSession(string title, string content) { }

        public void StopSession() { }

        public void TriggerHapticFeedback() { }
    }
}
