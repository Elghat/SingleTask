using SingleTask.Core.Services;
using System.Threading.Tasks;
using Microsoft.Maui.Devices;
using Android.Content;
using AndroidMedia = Android.Media;
using SingleTask.Core.Models;
using Microsoft.Maui.ApplicationModel;
using Android.Content.PM;
using AndroidX.Core.Content;
using Android;
using Android.App;

namespace SingleTask.Platforms.Android.Services
{
    public class AndroidFocusService : INativeFocusService, IFocusService
    {
        // IFocusService Implementation
        public TaskItem? CurrentFocusedTask { get; private set; }

        public void StartFocusSession(TaskItem task)
        {
            CurrentFocusedTask = task;
            StartSession(task.Title, "Focus Checkpoint");
        }

        public void UpdateFocusSession(TaskItem task)
        {
            CurrentFocusedTask = task;
            StopSession(); // Update notification by restarting for now, or just restarting service
            StartSession(task.Title, "Next: " + task.Title);
        }

        public void StopFocusSession()
        {
            CurrentFocusedTask = null;
            StopSession();
        }

        // INativeFocusService Implementation
        public Task<bool> CheckNotificationPermissionAsync()
        {
            if (global::Android.OS.Build.VERSION.SdkInt >= global::Android.OS.BuildVersionCodes.Tiramisu)
            {
                var status = ContextCompat.CheckSelfPermission(Platform.CurrentActivity!, Manifest.Permission.PostNotifications);
                return Task.FromResult(status == Permission.Granted);
            }
            return Task.FromResult(true);
        }

        public async Task PlaySuccessSoundAsync()
        {
            var context = Platform.CurrentActivity?.ApplicationContext;
            if (context == null) return;

            try
            {
                // Access MauiAsset via AssetManager
                using var fd = context.Assets?.OpenFd("Success_Bell.mp3");
                if (fd != null)
                {
                    var player = new AndroidMedia.MediaPlayer();
                    await player.SetDataSourceAsync(fd.FileDescriptor, fd.StartOffset, fd.Length);
                    player.Prepare();
                    player.Start();

                    player.Completion += (s, e) =>
                    {
                        player.Release();
                        player.Dispose();
                    };
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"Audio Playback Failed: {ex.Message}");
#endif
            }
        }

        public async Task RequestNotificationPermissionAsync()
        {
            if (global::Android.OS.Build.VERSION.SdkInt >= global::Android.OS.BuildVersionCodes.Tiramisu)
            {
                if (await Permissions.RequestAsync<Permissions.PostNotifications>() != PermissionStatus.Granted)
                {
                    // Handle denial or assume ViewModel handles it
                }
            }
        }

        public void StartSession(string title, string content)
        {
            var context = Platform.CurrentActivity;
            if (context == null) return;

            var intent = new Intent(context, typeof(FocusSessionService));
            intent.PutExtra("title", title);
            intent.PutExtra("content", content);

            // Intent extras will be added here
            if (global::Android.OS.Build.VERSION.SdkInt >= global::Android.OS.BuildVersionCodes.O)
            {
                context.StartForegroundService(intent);
            }
            else
            {
                context.StartService(intent);
            }
        }

        public void StopSession()
        {
            var context = Platform.CurrentActivity;
            if (context == null) return;

            var intent = new Intent(context, typeof(FocusSessionService));
            context.StopService(intent);
        }

        public void TriggerHapticFeedback()
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
        }
    }
}
