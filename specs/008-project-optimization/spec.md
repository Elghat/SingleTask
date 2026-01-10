# Feature Specification: Project Optimization Overhaul

**Feature Branch**: `008-project-optimization`  
**Created**: 2026-01-10  
**Status**: Draft  
**Input**: User description: "Implement all project optimization changes from the PROJECT_OPTIMIZATION_REPORT.md including async/await fixes, batch database operations, build configuration improvements, performance enhancements, and code quality improvements."

**Source Document**: `docs/PROJECT_OPTIMIZATION_REPORT.md`

---

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Stable Application Without Crash from Async Void (Priority: P1)

As a user running the SingleTask app, I expect the application to remain stable and not crash unexpectedly when completing tasks or navigating between pages.

**Why this priority**: `async void` methods cause unhandled exceptions that crash the entire application. This is the highest-impact bug fix affecting all users.

**Independent Test**: Can be validated by triggering error conditions in FocusViewModel.Initialize() and confirming the app handles them gracefully instead of crashing.

**Acceptance Scenarios**:

1. **Given** I am on the Focus page and an error occurs during initialization, **When** the error is thrown, **Then** the app displays an error message and remains functional (does not crash).
2. **Given** I complete a task and the database save fails, **When** the save operation throws an exception, **Then** the app handles the error gracefully and allows retry.
3. **Given** I navigate to the Celebration page and an animation error occurs, **When** the animation fails, **Then** the app continues to work without crashing.

---

### User Story 2 - Fast Task Reordering Experience (Priority: P1)

As a user with 10+ tasks, I expect task reordering (moving tasks up/down) to feel instant and responsive, without noticeable lag or freezing.

**Why this priority**: Current implementation performs N database writes per reorder operation, causing significant UI lag. This directly impacts the core user experience.

**Independent Test**: Can be tested by creating 20 tasks, reordering them repeatedly, and measuring/observing response time.

**Acceptance Scenarios**:

1. **Given** I have 20 tasks in my list, **When** I move a task up or down, **Then** the reorder completes in under 200ms (perceived as instant).
2. **Given** I rapidly reorder multiple tasks in succession, **When** each reorder command is issued, **Then** the UI remains responsive and reorderings are persisted correctly.
3. **Given** I reorder tasks and close the app immediately, **When** I reopen the app, **Then** all task orders are correctly preserved.

---

### User Story 3 - Smooth Scrolling Task List (Priority: P2)

As a user with a long task list (20+ items), I expect the task list to scroll smoothly at 60fps without jank or stuttering.

**Why this priority**: The current `BindableLayout` does not virtualize items, causing all tasks to be rendered in memory. This causes scroll jank on lower-end devices.

**Independent Test**: Can be tested by populating 50 tasks and scrolling rapidly on a mid-tier Android device.

**Acceptance Scenarios**:

1. **Given** I have 50 tasks in my list, **When** I scroll through the list quickly, **Then** the scroll animation is smooth (60fps).
2. **Given** I scroll to the bottom of a long list, **When** I scroll back to the top, **Then** there is no visible stutter or frame drops.
3. **Given** I have many tasks with varying content lengths, **When** scrolling through the list, **Then** items are recycled efficiently and memory usage remains stable.

---

### User Story 4 - Reduced APK Size for Faster Downloads (Priority: P2)

As a user downloading the app from the Play Store, I expect the app to download quickly and not consume excessive storage on my device.

**Why this priority**: Current estimated APK size includes a 3.2MB unused font file and dead code. Reducing this improves install conversion rates and user satisfaction.

**Independent Test**: Can be verified by comparing APK size before and after optimization.

**Acceptance Scenarios**:

1. **Given** the app is built for release, **When** I check the APK size, **Then** it is at least 30% smaller than the pre-optimization baseline.
2. **Given** the app uses fonts, **When** I inspect the APK contents, **Then** only fonts that are actively used are included.
3. **Given** the app is installed, **When** I check storage usage, **Then** it consumes less space than before optimization.

---

### User Story 5 - Faster App Cold Startup (Priority: P2)

As a user launching the app after a device restart or after the app was killed, I expect the app to launch quickly without a long loading delay.

**Why this priority**: Build configuration improvements (AOT, trimming, marshal methods) directly improve cold startup performance.

**Independent Test**: Can be measured by timing app launch to interactive UI on a mid-tier Android device.

**Acceptance Scenarios**:

1. **Given** the app is not in memory (cold start), **When** I tap the app icon, **Then** the app reaches interactive state within 2 seconds on a mid-tier device.
2. **Given** the app was previously closed, **When** I launch it again, **Then** there is no visible splash screen delay beyond the standard transition animation.

---

### User Story 6 - Reliable Audio Playback Without Freezing (Priority: P2)

As a user completing all my tasks and triggering the celebration, I expect the celebration sound to play without causing the app to freeze or stutter.

**Why this priority**: Current `Thread.Sleep` usage blocks the thread pool, potentially causing ANRs and UI freezes.

**Independent Test**: Can be tested by completing all tasks and observing UI responsiveness during celebration.

**Acceptance Scenarios**:

1. **Given** I complete my final task, **When** the celebration animation and sound plays, **Then** the UI remains responsive and I can tap buttons.
2. **Given** the celebration sound is playing, **When** I tap the dismiss button, **Then** the navigation responds immediately without delay.

---

### User Story 7 - Debuggable Production Issues (Priority: P3)

As a developer maintaining the app, I expect to be able to diagnose issues from production crash reports and logs.

**Why this priority**: Current silent `catch { }` blocks hide diagnostic information, making production debugging nearly impossible.

**Independent Test**: Can be validated by reviewing code for proper exception logging patterns.

**Acceptance Scenarios**:

1. **Given** an exception occurs in a catch block, **When** the exception is caught, **Then** it is logged with sufficient context (message, stack trace) in debug builds.
2. **Given** a non-critical exception occurs (e.g., haptic feedback fails), **When** the exception is caught, **Then** the app continues gracefully and the exception is logged.

---

### User Story 8 - Clean and Maintainable Codebase (Priority: P3)

As a developer working on the codebase, I expect unused code, files, and resources to be removed so the codebase is easier to navigate and maintain.

**Why this priority**: Dead code (MainPage, MainViewModel, TestEntity in Core, unused fonts) increases cognitive load and maintenance burden.

**Independent Test**: Can be validated by code review confirming removal of unused files.

**Acceptance Scenarios**:

1. **Given** I open the solution, **When** I browse the source files, **Then** there are no unused Page/ViewModel files.
2. **Given** I inspect the Resources/Fonts folder, **When** I list the fonts, **Then** only fonts that are actively used are present.
3. **Given** I search for orphaned log files in src/, **When** I list the directory, **Then** there are no `.log` or temp files.

---

### Edge Cases

- What happens when database batch save is interrupted mid-transaction?
  - System should rollback the transaction and preserve original order.
  
- What happens when a task is reordered while another reorder is in progress?
  - Rate limiting should debounce rapid commands to prevent race conditions.

- What happens if the font subsetting removes glyphs that are used?
  - Build validation should include a check for required character coverage.

- What happens if `CollectionView` virtualization breaks with certain item heights?
  - Test with variable-length task titles to ensure consistent behavior.

---

## Requirements *(mandatory)*

### Functional Requirements

#### High Priority - Code Quality & Stability

- **FR-001**: System MUST convert all `async void` methods to `async Task` with proper exception handling.
  - Affected: `FocusViewModel.Initialize()`, `PlanningPage.OnAppearing()`, `CelebrationPage.OnAppearing()`
  
- **FR-002**: System MUST replace all `Thread.Sleep()` calls with `await Task.Delay()` in async contexts.
  - Affected: `AudioService.PlayCelebrationSoundAsync()`
  
- **FR-003**: System MUST add exception logging to all silent `catch { }` blocks in debug builds.
  - Affected: `DatabaseService`, `HapticService`, `CelebrationPage`, `AudioService`
  
- **FR-004**: System MUST implement batch database operations for task order persistence.
  - Add `SaveTasksAsync(IEnumerable<TaskItem>)` to `IDatabaseService`
  - Wrap multiple saves in a single transaction

#### High Priority - Performance

- **FR-005**: System MUST replace `BindableLayout` with `CollectionView` for task list virtualization.
  - Affected: `PlanningPage.xaml` task list

#### Medium Priority - Build Configuration

- **FR-006**: System MUST add explicit `RunAOTCompilation=true` to Release build configuration.
  
- **FR-007**: System MUST add `TrimMode=full` to Release build configuration for .NET 10.
  
- **FR-008**: System MUST enable `AndroidEnableMarshalMethods=true` for Android performance optimization.

- **FR-009**: System MUST add `AndroidEnablePreloadAssemblies=true` for startup tracing.

#### Medium Priority - Resource Optimization

- **FR-010**: System MUST remove unused font files (`OpenSans-Regular.ttf`, `OpenSans-Semibold.ttf`).

- **FR-011**: System MUST remove or archive unused source files:
  - `MainPage.xaml` / `MainPage.xaml.cs`
  - `MainViewModel.cs`
  - Move `TestEntity.cs` to test project if needed

- **FR-012**: System MUST add orphaned build artifacts to `.gitignore`:
  - `*.log` files
  - `preprocessed.xml`

- **FR-013**: System MUST optimize or subset the large font file (`MPLUSRounded1c-Regular.ttf` at 3.2MB).

#### Medium Priority - Code Quality

- **FR-014**: System MUST implement `IAsyncDisposable` on `DatabaseService` to properly release database connections.

- **FR-015**: System MUST dispose `ToneGenerator` instances in `AudioService` using proper `using` statements.

#### Low Priority - Android Integration

- **FR-016**: System SHOULD consolidate duplicate foreground service implementations (`FocusForegroundService.cs` and `FocusSessionService.cs`).

- **FR-017**: System SHOULD create a custom notification icon to replace the generic `dotnet_bot` drawable.

#### Low Priority - Standardization

- **FR-018**: System SHOULD standardize on `[ObservableProperty]` attribute throughout all ViewModels (convert `MainViewModel.CountText` manual property).

---

### Key Entities

- **TaskItem**: Core task entity with Order, Status, Title. Affected by batch save optimization.
- **DatabaseService**: Singleton database service. Requires new batch method and IAsyncDisposable.
- **AudioService**: Platform audio service. Requires async pattern fixes.
- **FocusViewModel**: Focus mode controller. Requires async void fix.

---

## Success Criteria *(mandatory)*

### Measurable Outcomes

#### Performance

- **SC-001**: Task reordering operations complete in under 200ms for lists of 20+ tasks (currently ~2-3 seconds with 20 tasks).

- **SC-002**: Task list scrolls at 60fps with no dropped frames for lists of 50+ items.

- **SC-003**: Cold app startup time is reduced by at least 200ms compared to pre-optimization baseline on a mid-tier Android device.

- **SC-004**: Celebration sound playback does not block UI thread (no ANRs or frame drops during playback).

#### Size Reduction

- **SC-005**: Release APK size is reduced by at least 30% compared to pre-optimization baseline (~2-3MB reduction expected).

- **SC-006**: Unused code and resources are removed, reducing total source files by at least 5 files.

#### Stability

- **SC-007**: Zero app crashes caused by unhandled exceptions in async methods under normal usage scenarios.

- **SC-008**: 100% of catch blocks contain appropriate logging in debug builds (verified by code review).

#### Maintainability

- **SC-009**: All high-priority functional requirements (FR-001 through FR-005) are implemented and verified by unit tests where applicable.

- **SC-010**: Build configuration changes do not introduce regression in existing functionality (verified by running existing test suite).

---

## Assumptions

1. The project targets .NET 10.0 with Android API 35 (already in place).
2. The existing test suite passes before optimization changes begin.
3. Font subsetting will use command-line tools (e.g., `fonttools`) or manual reduction.
4. No new dependencies will be added for these optimizations (using existing SDK capabilities).
5. Baseline measurements for APK size and startup time will be captured before implementation.

---

## Out of Scope

1. iOS implementation (project currently targets Android only)
2. Localization infrastructure (mentioned in report but deferred)
3. New unit test creation beyond what's needed to verify the optimization changes
4. UI/UX redesign or feature additions
5. Third-party performance monitoring integration
