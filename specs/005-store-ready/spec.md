# Feature Specification: Google Play Store Preparation

**Feature Branch**: `005-store-ready`
**Created**: 2026-01-04
**Status**: Draft
**Input**: User description provided.

## User Scenarios & Testing

### User Story 1 - Brand Identity Verification (Priority: P1)
As a product owner, I want the app to display the correct icon and splash screen so that it aligns with the "Warm Paper" brand on the user's device.

**Independent Test**: Install app on Android 12+ device (emulator or physical). Verify app icon on home screen. Launch app and verify splash screen background color `#fdfbf7` and icon.

**Acceptance Scenarios**:
1. **Given** the app is installed, **When** viewing the home screen, **Then** the app icon matches `AppIcon.svg`.
2. **Given** the app is launching, **When** the splash screen appears, **Then** the background color is `#fdfbf7` and the branding is visible.
3. **Given** the app is installed, **When** viewing the app name, **Then** it displays "SingleTask".

### User Story 2 - Release Versioning (Priority: P1)
As a release manager, I want the app to have the correct version numbers and ID so that it can be uploaded to the Google Play Store without conflict.

**Independent Test**: Inspect `AndroidManifest.xml` (or build output) for `versionCode` "1", `versionName` "1.0.0", and package `com.kingjo.singletask`.

**Acceptance Scenarios**:
1. **Given** the build is configured for Release, **When** inspecting the generated manifest, **Then** `package` is `com.kingjo.singletask`.
2. **Given** the build is configured, **When** checking version, **Then** `versionName` is "1.0.0" and `versionCode` is "1".

### User Story 3 - Code Hygiene (Priority: P2)
As a developer, I want the codebase to be free of debug logs and temporary markers so that the release build is professional and performant.

**Independent Test**: Grep codebase for `Console.WriteLine` and `TODO`.

**Acceptance Scenarios**:
1. **Given** the release build, **When** code is scanned, **Then** no `System.Console.WriteLine` or `Debug.WriteLine` calls exist (except explicitly allowed logging).
2. **Given** the release build, **When** code is scanned, **Then** `TODO` and `FIXME` comments are identified and reported.

## Requirements

### Functional Requirements
- **FR-001**: System MUST display the application name as "SingleTask" on the device.
- **FR-002**: System MUST use `AppIcon.svg` for the adaptive application icon.
- **FR-003**: System MUST use `MauiSplashScreen` with background color `#fdfbf7`.
- **FR-004**: System MUST have Application ID `com.kingjo.singletask`.
- **FR-005**: System MUST have Application Display Version "1.0.0".
- **FR-006**: System MUST have Application Version "1".
- **FR-007**: System MUST NOT contain `System.Console.WriteLine` or `Debug.WriteLine` in the release codebase.
- **FR-008**: System MUST report all `TODO` and `FIXME` markers remaining in the code.

### Success Criteria
- **SC-001**: App installs on Android 12+ with correct Icon and Splash Screen.
- **SC-002**: Package name is `com.kingjo.singletask`.
- **SC-003**: Version is 1.0.0 (1).
- **SC-004**: Zero debug print statements in source code.

## Assumptions
- The `AppIcon.svg` and `splash.svg` files exist or placeholders can be created.
- The build process handles the standard MAUI `csproj` configuration.
