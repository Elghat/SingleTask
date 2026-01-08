# Feature Specification: Native Integration (Notification & Sensory Feedback)

**Feature Branch**: `004-native-features`
**Created**: 2026-01-04
**Status**: Draft
**Input**: User description: "Native Integration... prevent OS from killing focus... sensory feedback... NO TIMER... Success_Bell.mp3... Handle permissions gracefully... Force rigorous testing."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Persistent Focus Session (Priority: P1)

As a user, I want my Focus Session to remain active and retrievable even if I switch apps, so that I don't lose my focus state due to the OS reclaiming resources.

**Why this priority**: Critical. Without a Foreground Service, Android may kill the app process when in the background, losing the user's current session state.

**Independent Test**:
*   **Step 1**: Start a Focus Session.
*   **Step 2**: Minimize the app and open 3-4 other memory-intensive apps (e.g., Maps, Camera, Chrome).
*   **Step 3**: Wait 5 minutes.
*   **Step 4**: Return to the app.
*   **Verification**: The app must resume *immediately* in the Focus state without reloading from scratch (splash screen).

**Acceptance Scenarios**:

1.  **Given** a Focus Session is active, **When** I background the app, **Then** a Foreground Service starts (visible via system notification if permission granted).
2.  **Given** the Foreground Service is running, **When** I return to the app, **Then** the session is still active.
3.  **Given** the session ends, **Then** the Foreground Service stops and the notification clears.

---

### User Story 2 - Permission-Aware Notifications (Priority: P2)

As a user, I want the app to respect my notification preferences. If I grant permission, I see the persistence notification. If I deny it, the app keeps working without crashing or pestering me.

**Why this priority**: Compliance with Android 13+ and good UX. Crashing on permission denial is unacceptable.

**Independent Test**:
*   **Test A (Granted)**: Install app, Grant Permissions, Start Focus -> Verify Notification shows.
*   **Test B (Denied)**: Uninstall/Reinstall, **DENY** Permissions, Start Focus -> **Verify NO crash**, NO notification, but app state remains active if possible (OS dependent).

**Acceptance Scenarios**:

1.  **Given** permission is NOT granted, **When** Focus starts, **Then** the code explicitly checks permission, finds it missing, and **skips** the notification builder logic (does NOT crash).
2.  **Given** permission IS granted, **When** Focus starts, **Then** the "Focusing on [Task]" notification appears.

---

### User Story 3 - Sensory Feedback (Juice) (Priority: P2)

As a user, I want clear physical and auditory feedback when I complete tasks to feel a sense of accomplishment.

**Why this priority**: "Game feel" / Dopamine hit.

**Independent Test**:
*   **Step 1**: Complete a task.
*   **Verification**: Phone MUST vibrate AND play `Success_Bell.mp3`.
*   **Step 2**: Start Focus Mode.
*   **Verification**: Phone MUST vibrate (NO sound).
*   **Step 3**: Stop Focus Mode.
*   **Verification**: Phone MUST vibrate (NO sound).

**Acceptance Scenarios**:

1.  **Given** a task is completed, **Then** play `Success_Bell.mp3`.
2.  **Given** a task is completed, **Then** trigger `HapticFeedback.Click`.
3.  **Given** focus starts/stops, **Then** trigger `HapticFeedback.Click` (BUT NO SOUND).

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST implement a platform-specific background service to keep the session active during Focus Mode.
- **FR-002**: System MUST check for notification permissions (on supported platforms) before attempting to show any status notification.
- **FR-003**: If permission is DENIED, the system MUST gracefully skip notification creation and continue the session logic without crashing.
- **FR-004**: If permission is GRANTED, the system MUST show a persistent notification with title "Focusing on [Task Name]" and text "Stay focused!".
- **FR-005**: Tapping the notification MUST bring the application to the foreground and resume the active session page.
- **FR-006**: System MUST play the success audio asset (`Success_Bell.mp3`) **ONLY** when a task is marked as completed.
- **FR-007**: System MUST trigger standard haptic feedback on: Task Completion, Focus Start, and Focus Stop.
- **FR-008**: The background service MUST stop immediately when the Focus Session ends.

### Key Entities

- **FocusSessionService**: Manages the lifecycle of the background process and notification.
- **PermissionHelper**: Utility to check/request permissions safely.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: **Zero Crashes** when running on platforms with strictly enforced notification permissions (e.g., Android 13+) when permissions are disabled.
- **SC-002**: App process remains alive and responsive for >10 minutes in background during a Focus Session (verified via system process tools).
- **SC-003**: Success audio plays within 200ms of task completion event.
- **SC-004**: Haptics trigger 100% of the time for specified actions.

## Assumptions

- The file `Success_Bell.mp3` exists in the project root or `Resources/Raw`.
- The user is on a device that supports haptic feedback.

## Technical Constraints

- **Platform**: Android Target (Foreground Service).
- **Permissions**: Must handle `POST_NOTIFICATIONS` (Android 13+) and `FOREGROUND_SERVICE`.
- **Architecture**: Must use Dependency Injection for platform-specific code (e.g., `INativeFocusService`).
- **Strict Error Handling**: Any permission security exception MUST be caught.
- **Resource Management**: Audio resources MUST be released after playback.
