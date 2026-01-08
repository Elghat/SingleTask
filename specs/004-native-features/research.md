# Research: Native Integration (Notification & Sensory Feedback)

**Feature**: Native Integration (Notification & Sensory Feedback)
**Status**: In Progress

## 1. Technical Clarifications (NEEDS CLARIFICATION)

### Unknown 1: Android Foreground Service Implementation in MAUI
- **Question**: How to implement a `ForegroundService` in .NET MAUI targeting Android 13+?
- **Research**:
  - Needs a class inheriting `Service`.
  - Must be registered in `AndroidManifest.xml`.
  - Requires `StartForegroundService` intent.
  - Needs a `NotificationChannel` (Android 8.0+).
- **Decision**: Create `FocusSessionService` in `Platforms/Android`. Expose via `INativeFocusService`.

### Unknown 2: Permission Handling (Android 13/TPM 33+)
- **Question**: How to gracefully handle `POST_NOTIFICATIONS` denial?
- **Research**:
  - `Permissions.RequestAsync<Permissions.PostNotifications>()` (available in MAUI Essentials?).
  - If denied, `StartForeground` might crash on some devices if not handled carefully, OR it just runs without notification icon in drawer (but strict mode might throw).
  - **Crucial**: Android 14 mandates strict types for Foreground Services.
  - **Constraint**: Spec says "If denied... skip notification builder... do not crash".
  - **Refinement**: A Foreground Service *MUST* have a notification. You cannot start a Foreground Service without one.
  - **Resolution**: If permission is denied, we **CANNOT start the Foreground Service**. The requirement "skip notification... continue session" implies we just run as a standard background app (which might be killed). We will respect the "No Crash" rule by wrapping the service start call in a check.

### Unknown 3: Audio Playback (`Success_Bell.mp3`)
- **Question**: Best way to play a short SFX in MAUI without overhead?
- **Research**:
  - `Plugin.Maui.Audio` is popular but might be overkill.
  - Native `MediaPlayer` (Android) / `AVAudioPlayer` (iOS) via Dependency Injection is lighter and strictly native as per "Native Integration" theme.
  - **Decision**: Use `Android.Media.MediaPlayer` directly in `AndroidFocusService` or a separate `AudioService` (if shared later, but feature scope focuses on Native Integration). The spec mentions `INativeFocusService` - we can add `PlaySuccessSound()` there.

## 2. Technology Choices

| Component | Choice | Rationale |
| :--- | :--- | :--- |
| **Service** | Native `android.app.Service` | Required for `startForeground` to keep app alive. |
| **Messaging** | `IMessenger` (WeakReferenceMessenger) | To decouple ViewModel from Service start/stop signals. |
| **Audio** | Native `MediaPlayer` (Android) | Low latency, no extra NuGet deps if possible, or `Plugin.Maui.Audio` if native is too verbose. *Decision: Native for now to minimize deps.* |
| **Haptics** | `Microsoft.Maui.Devices.HapticFeedback` | Built-in MAUI API, covers standard "Click". |

## 3. Architecture & Patterns

### Service Lifecycle
1. **Start**: `FocusViewModel` -> `INativeFocusService.StartFocusSession(taskTitle)`.
   - Android impl: Starts `FocusSessionService`.
   - Service: Creates Notification -> `StartForeground`.
2. **Update**: `FocusViewModel` (Timer Tick) -> `INativeFocusService.UpdateStatus(timeLeft)`.
   - Service: Updates Notification content.
3. **Stop**: `FocusViewModel` -> `INativeFocusService.StopFocusSession()`.
   - Service: `StopForeground(true)` -> `StopSelf()`.

### Dependency Injection
- `MauiProgram.cs`:
  ```csharp
  #if ANDROID
  builder.Services.AddSingleton<INativeFocusService, AndroidFocusService>();
  #endif
  ```

### Asset Location
- `Success_Bell.mp3` -> `Resources/Raw/Success_Bell.mp3`.
- Android Load: `Android.App.Application.Context.Resources.GetIdentifier...` or `Assets.OpenFd`.

## 4. Risks & Mitigations
- **Risk**: Android 14+ specific foreground service types.
  - **Mitigation**: Declare `foregroundServiceType="specialUse"` or `dataSync` in Manifest. *Wait, for a timer?* "shortService" (API 34)? No, standard foreground service is fine but needs type in Manifest.
- **Risk**: Service getting killed anyway.
  - **Mitigation**: Use `START_STICKY`.

