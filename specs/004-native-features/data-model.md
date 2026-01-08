# Data Model: Native Integration

**Feature**: `004-native-features`

## Interfaces

### `INativeFocusService`
*Located in `SingleTask.Core.Services`*

Abstracts platform-specific background execution and sensory feedback.

```csharp
public interface INativeFocusService
{
    /// <summary>
    /// Starts the native background session (Foreground Service on Android).
    /// </summary>
    /// <param name="taskTitle">The name of the current task to display in notification.</param>
    /// <param name="description">Additional context (e.g., "Stay focused!").</param>
    void StartSession(string taskTitle, string description);

    /// <summary>
    /// Stops the native background session and clears notifications.
    /// </summary>
    void StopSession();

    /// <summary>
    /// Plays the success sound effect (Success_Bell.mp3).
    /// </summary>
    Task PlaySuccessSoundAsync();

    /// <summary>
    /// Triggers a haptic feedback pattern.
    /// </summary>
    void TriggerHapticFeedback();
    
    /// <summary>
    /// Checks if the app has permission to show notifications (Android 13+).
    /// </summary>
    Task<bool> CheckNotificationPermissionAsync();
    
    /// <summary>
    /// Requests notification permission.
    /// </summary>
    Task<bool> RequestNotificationPermissionAsync();
}
```

## Android Implementation

### `FocusSessionService`
*Located in `SingleTask.Platforms.Android.Services`*

Native Android Service to maintain foreground priority.

**Attributes:**
- `[Service]`
- `ForegroundServiceType` (in Manifest)

**Methods:**
- `OnStartCommand(Intent, Flags, StartId)`: Handles Start/Stop commands.
- `CreateNotificationChannel()`: Sets up the channel for persistent notifications.
- `BuildNotification()`: Constructs the UI for the notification.

### `AndroidFocusService`
*Located in `SingleTask.Platforms.Android.Services`*

Implementation of `INativeFocusService`.

**Responsibilities:**
- Marshals calls from Core to the `FocusSessionService` via `Intent`.
- Handles `MediaPlayer` logic for audio.
- Wraps `HapticFeedback` API.
- Wraps `Permissions` API for notifications.

## Assets

- `Resources/Raw/Success_Bell.mp3`: Audio file for success feedback.
