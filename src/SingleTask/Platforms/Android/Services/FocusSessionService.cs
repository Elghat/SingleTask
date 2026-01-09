using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using AndroidX.Core.App;
using Android.Content.PM;

namespace SingleTask.Platforms.Android.Services
{
    // SEC-004: Explicitly mark internal service as not exported
    [Service(Exported = false, ForegroundServiceType = ForegroundService.TypeSpecialUse)]
    public class FocusSessionService : Service
    {
        private const string CHANNEL_ID = "focus_channel";
        private const string CHANNEL_NAME = "Focus Session";
        private const int NOTIFICATION_ID = 1001;

        public override IBinder? OnBind(Intent? intent)
        {
            return null;
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent? intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            var title = intent?.GetStringExtra("title") ?? "Focus Mode";
            var content = intent?.GetStringExtra("content") ?? "Stay focused on your task.";

            CreateNotificationChannel();

            // SEC-009: Use explicit intent construction instead of GetLaunchIntentForPackage
            var launchIntent = new Intent(this, typeof(MainActivity));
            launchIntent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(
                this,
                0,
                launchIntent,
                PendingIntentFlags.Immutable);

            var notification = new NotificationCompat.Builder(this, CHANNEL_ID)
                .SetContentTitle(title)
                .SetContentText(content)
                .SetSmallIcon(Resource.Drawable.ic_start) // Using existing drawable
                .SetContentIntent(pendingIntent)
                .SetOngoing(true)
                .Build();

            // API 34+ requires specifying the type
            if (Build.VERSION.SdkInt >= BuildVersionCodes.UpsideDownCake)
            {
                StartForeground(NOTIFICATION_ID, notification, ForegroundService.TypeSpecialUse);
            }
            else if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
            {
                StartForeground(NOTIFICATION_ID, notification, ForegroundService.TypeManifest);
            }
            else
            {
                StartForeground(NOTIFICATION_ID, notification);
            }

            return StartCommandResult.Sticky;
        }

        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = new NotificationChannel(CHANNEL_ID, CHANNEL_NAME, NotificationImportance.Low)
                {
                    Description = "Active Focus Session Notifications"
                };

                var notificationManager = (NotificationManager)GetSystemService(NotificationService)!;
                notificationManager.CreateNotificationChannel(channel);
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            StopForeground(StopForegroundFlags.Remove);
        }
    }
}
