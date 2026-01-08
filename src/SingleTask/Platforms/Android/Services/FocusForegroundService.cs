using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using AndroidX.Core.App;
using SingleTask.Core.Models;

namespace SingleTask.Platforms.Android.Services;

[Service(Exported = false, ForegroundServiceType = global::Android.Content.PM.ForegroundService.TypeDataSync)]
public class FocusForegroundService : Service
{
    public const string NOTIFICATION_CHANNEL_ID = "focus_channel";
    public const int NOTIFICATION_ID = 1337;
    public const string EXTRA_TASK_TITLE = "extra_task_title";

    public override IBinder? OnBind(Intent? intent)
    {
        return null;
    }

    public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
    {
        if (intent?.Action == "STOP_SERVICE")
        {
            StopForeground(StopForegroundFlags.Remove);
            StopSelfResult(startId);
            return StartCommandResult.NotSticky;
        }

        var taskTitle = intent?.GetStringExtra(EXTRA_TASK_TITLE) ?? "Focusing...";
        CreateNotificationChannel();

        var notification = new NotificationCompat.Builder(this, NOTIFICATION_CHANNEL_ID)
            .SetContentTitle("Standard Focus")
            .SetContentText(taskTitle)
            .SetSmallIcon(Resource.Drawable.dotnet_bot) // Generic icon to ensure build success
            .SetPriority(NotificationCompat.PriorityHigh)
            .SetOngoing(true)
            .Build();

        StartForeground(NOTIFICATION_ID, notification);

        return StartCommandResult.Sticky;
    }

    private void CreateNotificationChannel()
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            var channel = new NotificationChannel(NOTIFICATION_CHANNEL_ID, "Focus Session", NotificationImportance.High)
            {
                Description = "Ongoing Focus Session"
            };

            var notificationManager = (NotificationManager)GetSystemService(NotificationService)!;
            notificationManager.CreateNotificationChannel(channel);
        }
    }
}
