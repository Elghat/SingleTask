# Tasks: Native Integration

**Feature**: `004-native-features`
**Plan**: [specs/004-native-features/plan.md](../plan.md)

## Phase 1: Infrastructure & Assets

- [ ] **Asset Setup** <!-- id: 0 -->
  - Locate `Success_Bell.mp3` in root (or create dummy if missing for now).
  - Move/Copy to `src/SingleTask/Resources/Raw/Success_Bell.mp3`.
  - Ensure `.csproj` includes it as `MauiAsset`.
- [ ] **Core Interfaces** <!-- id: 1 -->
  - Create `src/SingleTask.Core/Services/INativeFocusService.cs`.
  - Define methods: `StartSession`, `StopSession`, `PlaySuccessSoundAsync`, `TriggerHapticFeedback`, `CheckNotificationPermissionAsync`, `RequestNotificationPermissionAsync`.

## Phase 2: Android Implementation

- [ ] **Manifest Configuration** <!-- id: 2 -->
  - Edit `src/SingleTask/Platforms/Android/AndroidManifest.xml`.
  - Add `<uses-permission android:name="android.permission.FOREGROUND_SERVICE" />`.
  - Add `<uses-permission android:name="android.permission.POST_NOTIFICATIONS" />`.
  - Add `<uses-permission android:name="android.permission.FOREGROUND_SERVICE_SPECIAL_USE" />` (or appropriate type for Android 14 target).
- [ ] **Native Service Class** <!-- id: 3 -->
  - Create `src/SingleTask/Platforms/Android/Services/FocusSessionService.cs`.
  - Inherit `Android.App.Service`.
  - Implement `OnStartCommand` to build notification and call `StartForeground`.
  - Handle `OnDestroy` to clean up.
- [ ] **Service Bridge Class** <!-- id: 4 -->
  - Create `src/SingleTask/Platforms/Android/Services/AndroidFocusService.cs`.
  - Implement `INativeFocusService`.
  - Use `Platform.CurrentActivity` context for intents.
  - Implement Audio using `Android.Media.MediaPlayer`.
  - Implement Haptics using `Microsoft.Maui.Devices.HapticFeedback`.
- [ ] **Dependency Injection** <!-- id: 5 -->
  - Update `src/SingleTask/MauiProgram.cs`.
  - Add `#if ANDROID` block.
  - Register `INativeFocusService` -> `AndroidFocusService` as Singleton.
  - Add `AddTransient` for `FocusSessionService` (if needed, usually system manages it).

## Phase 3: Integration

- [ ] **ViewModel Logic** <!-- id: 6 -->
  - Update `FocusViewModel.cs`.
  - Inject `INativeFocusService`.
  - Update `StartFocusCommand`: Check Perms -> Start Service -> Trigger Haptic.
  - Update `StopFocusCommand`: Stop Service -> Trigger Haptic.
  - Update `CompleteTaskCommand`: Play Audio -> Trigger Haptic.

## Phase 4: Testing

- [ ] **Unit Tests** <!-- id: 7 -->
  - Create `NativeServiceTests.cs` in `SingleTask.UnitTests`.
  - Test `FocusViewModel` interaction with Mock `INativeFocusService`.
- [ ] **Verification** <!-- id: 8 -->
  - Run on Android Emulator (API 33+).
  - Verify Notification behavior.
  - Verify "No Permission" scenario.
