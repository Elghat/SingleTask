# Implementation Plan: Google Play Store Preparation

**Branch**: `005-store-ready` | **Date**: 2026-01-04 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `/specs/005-store-ready/spec.md`

## Summary
Prepare the application for the Google Play Store by configuring the Application ID, Versioning, App Icon, Splash Screen (Android 12+ compatible), and cleaning up debug code.

## Technical Context
**Language/Version**: C# (.NET 8 MAUI)
**Primary Dependencies**: MAUI Android
**Target Platform**: Android 12+ (API 31+)
**Project Type**: Mobile App
**Constraints**: Release configuration must be clean.

## Constitution Check
*   **Test-First**: Manual verification steps defined in Spec and Quickstart.
*   **Simplicity**: Using standard MAUI configuration mechanisms.

## Project Structure
### Documentation
```text
specs/005-store-ready/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
└── tasks.md
```

### Source Code
```text
src/SingleTask/
├── SingleTask.csproj        # PRIMARY TARGET: Version, AppId, Splash Config
├── Platforms/Android/
│   ├── Services/
│   │   ├── AudioService.cs        # Cleanup Debug.WriteLine
│   │   └── AndroidFocusService.cs # Cleanup Debug.WriteLine
│   └── MainApplication.cs         # Verify Attributes
└── Resources/
    ├── AppIcon/
    │   └── appicon.svg      # Verify existence/usage
    └── Splash/
        └── splash.svg       # Verify existence/usage
```

## Phases

### Phase 1: Configuration & Assets
*   **Goal**: Ensure `csproj` has correct metadata and asset references.
*   **Verification**: `dotnet build` produces manifest with correct package name/version.
*   **Files**: `SingleTask.csproj`.

### Phase 2: Code Hygiene
*   **Goal**: Remove debug logs from Release build.
*   **Strategy**: Wrap `Debug.WriteLine` in `#if DEBUG` preprocessor directives.
*   **Files**: `AudioService.cs`, `AndroidFocusService.cs`.

### Phase 3: Final Verification
*   **Goal**: Confirm visual branding on device.
*   **Action**: Deploy Release build to emulator/device.

## Complexity Tracking
None. Standard configuration.
