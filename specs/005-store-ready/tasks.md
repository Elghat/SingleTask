# Tasks: Google Play Store Preparation

**Feature Branch**: `005-store-ready`
**Status**: Ready
**Total Tasks**: 14

## Phase 1: Setup & Validation
**Goal**: Ensure all necessary assets are present before configuration.
**Independent Test**: All tasks checked off.

- [ ] T001 Verify existence of `src/SingleTask/Resources/AppIcon/appicon.svg`
- [ ] T002 Verify existence of `src/SingleTask/Resources/Splash/splash.svg`

## Phase 2: User Story 1 - Brand Identity (P1)
**Goal**: Configure App Icon, Splash Screen, and Display Name.
**Independent Test**: Build app and verify visual branding on emulator/device.

- [ ] T003 [US1] Update `src/SingleTask/SingleTask.csproj` to include `<MauiSplashScreen>` with `Color="#fdfbf7"` and `BaseSize="128,128"`
- [ ] T004 [US1] Ensure `src/SingleTask/SingleTask.csproj` has `ApplicationTitle` set to "SingleTask"
- [ ] T005 [US1] Verify `src/SingleTask/SingleTask.csproj` refers to `Resources\AppIcon\appicon.svg` for `<MauiIcon>`
- [ ] T006 [US1] Test: Build Release and verify Splash Screen Color and Icon visually on Android Emulator (Launch App)

## Phase 3: User Story 2 - Release Versioning (P1)
**Goal**: Set Application ID and Version numbers for Store Release.
**Independent Test**: Inspect generated `AndroidManifest.xml` for correct values.

- [ ] T007 [US2] Update `src/SingleTask/SingleTask.csproj` setting `ApplicationId` to `com.kingjo.singletask`
- [ ] T008 [US2] Update `src/SingleTask/SingleTask.csproj` setting `ApplicationDisplayVersion` to `1.0.0` and `ApplicationVersion` to `1`
- [ ] T009 [US2] Test: Build Release and inspect `src/SingleTask/obj/Release/net8.0-android/AndroidManifest.xml` for package name and version

## Phase 4: User Story 3 - Code Hygiene (P2)
**Goal**: Ensure Release build is free of debug logs.
**Independent Test**: Grep codebase for forbidden strings.

- [ ] T010 [P] [US3] Wrap `System.Diagnostics.Debug.WriteLine` in `#if DEBUG` block in `src/SingleTask/Platforms/Android/Services/AudioService.cs`
- [ ] T011 [P] [US3] Wrap `System.Diagnostics.Debug.WriteLine` in `#if DEBUG` block in `src/SingleTask/Platforms/Android/Services/AndroidFocusService.cs`
- [ ] T012 [US3] Test: Run `grep -r "Debug.WriteLine" src/SingleTask` to ensure no unwrapped calls remain
- [ ] T014 [US3] Report TODO and FIXME markers: Run `grep -r "TODO\|FIXME" src/SingleTask`

## Phase 5: Final Polish
**Goal**: Final full verification.

- [ ] T013 Deploy Release build to Android Emulator and perform full "Happy Path" test (Launch -> Splash -> Home -> Name Check)

## Implementation Strategy
1.  **MVP Scope**: Complete Phase 2 & 3 first (Core Store Requirements).
2.  **Incremental Delivery**: Configuration changes (US1/US2) are safe and low risk. Code changes (US3) are isolated to Android services.
3.  **Verification**: Heavy reliance on manual visual verification for US1, file inspection for US2, and grep for US3.

## Dependencies
- US1 & US2 can be done in parallel (modifying same file `SingleTask.csproj` - careful with merge, but logically distinct).
- US3 is independent.