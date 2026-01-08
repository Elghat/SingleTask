# Feature Specification: SingleTask Focus App

**Feature Branch**: `1-singletask-focus-app`  
**Created**: 2025-12-11  
**Status**: Draft  
**Input**: User description: "I want to create an app whose name is SingleTask the app consist of entering a series of task for the day and then display a permanent notification on the screen of the user showing the ongoing task When the user click on a go button the app would only display the ongoing task and maybe a counter of the remaining one but not the title of the remaining one i want the user to only focus on the ongoing one"

## Clarifications

### Session 2025-12-11
- Q: Application Platform → A: Mobile (Android MVP, iOS future) - "Permanent notification" means persistent notification in notification shade/lock screen, "always-on-top window" means dedicated full-screen Focus Mode within the app.
- Q: Animation Style → A: Playful (Juicy) - Confetti particles, bouncy transitions, pop effects.
- Q: "Mega Juicy" Animation for Daily Completion → A: Full Screen/Overlay Celebration: Temporary full-screen or large overlay with heavy confetti, trophy animation, and celebratory text.
- Q: Focus Mode Interaction → A: Minimal (Done only) - The persistent notification and Focus Mode screen offer only a "Done" or "Complete" action for the current task. No "Skip", "Edit", or "Add Task" from this mode.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Daily Task Planning (Priority: P1)

The user launches the application and enters a list of tasks they intend to accomplish for the day. This serves as the "planning phase" before entering focus mode.

**Why this priority**: Essential for populating the application with data. Without tasks, the focus mode has no purpose.

**Independent Test**: Can be tested by launching the app, adding multiple items to the list, editing them, and verifying the list is retained.

**Acceptance Scenarios**:

1. **Given** the app is open in "Planning Mode", **When** the user types a task title and presses Enter, **Then** the task is added to the visible list.
2. **Given** a list of tasks, **When** the user deletes a task, **Then** it is removed from the list.
3. **Given** a list of tasks, **When** the user reorders them (if supported, assuming basic list for now), **Then** the order is updated. (MVP: Just adding/deleting is sufficient).

---

### User Story 2 - Focus Mode (Permanent Widget) (Priority: P1)

After planning, the user activates "Focus Mode". The main application window hides or minimizes, and a small, permanent, always-on-top overlay appears showing *only* the current task.

**Why this priority**: This is the core unique value proposition (UVP) of the app—forcing focus by hiding distractions.

**Independent Test**: Can be tested by adding tasks, clicking "Go", and verifying the main window disappears and the widget appears with the correct content.

**Acceptance Scenarios**:

1. **Given** a non-empty task list, **When** the user clicks the "Go" button, **Then** the main window is hidden/minimizes.
2. **Given** "Focus Mode" is active, **When** the user looks at the screen, **Then** the dedicated full-screen 'Focus Mode' displays the title of the first incomplete task.
3. **Given** "Focus Mode" is active, **When** in 'Focus Mode', **Then** they see a counter of remaining tasks (e.g., "+3 more") but *not* the titles of those remaining tasks.

---

### User Story 3 - Task Progression (Priority: P1)

The user completes the current task from the persistent notification or Focus Mode screen and immediately sees the next one, maintaining flow without returning to the planning list.

**Why this priority**: completes the cycle of "Single Task" workflow.

**Independent Test**: Can be tested by completing tasks one by one in the widget and verifying the displayed text changes.

**Acceptance Scenarios**:

1. **Given** Focus Mode is active (either via persistent notification or dedicated screen) with Task A (current) and Task B (next), **When** the user clicks "Done" (or similar checkmark), **Then** Task A is marked complete with a playful animation (particles/pop) and Task B bounces/slides in as the displayed task.
2. **Given** the focus widget is active with the last task, **When** the user marks it complete, **Then** a temporary full-screen or large overlay appears with heavy confetti, trophy animation, and celebratory text before offering to return to Planning Mode/Exit.

### Edge Cases

- **Empty List**: What happens when "Go" is clicked with 0 tasks? (Should be disabled or show error).
- **App Restart**: If the app is closed during Focus Mode, does it restore the state? (FR: Data Persistence).
- **Long Titles**: How does the small widget handle very long task titles? (Text wrapping or truncation).

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST allow users to add, remove, and reorder tasks in a "Planning View".
- **FR-002**: System MUST persist the task list and current state between application restarts.
- **FR-003**: System MUST provide a "Go" / "Focus" trigger that switches the app to a "Focus Mode" screen and activates a persistent notification.
- **FR-004**: The app MUST display a persistent notification showing the current task and progress, and the 'Focus Mode' screen MUST be the primary view when active.
- **FR-005**: The "Focus Mode screen" MUST display the title of the *current* active task.
- **FR-006**: The "Focus Mode screen" MUST display a count of remaining pending tasks.
- **FR-007**: The "Focus Mode screen" MUST NOT display the titles of any upcoming/pending tasks.
- **FR-008**: The persistent notification AND the 'Focus Mode' screen MUST provide a mechanism to mark the current task as complete.
- **FR-009**: Upon completing a task, the system MUST automatically display the next pending task in the queue.
- **FR-010**: When all tasks are complete, the system MUST provide visual feedback and an option to return to the Planning View.
- **FR-011**: Task completion actions MUST trigger "juicy" visual feedback (e.g., confetti particles, scale/pop effects).
- **FR-012**: Upon completion of all daily tasks, the system MUST trigger a "mega juicy" full-screen/large overlay celebration with heavy confetti, trophy animation, and celebratory text.

### Key Entities

- **Task**: Represents a unit of work.
  - `id`: Unique identifier
  - `title`: String (Description of work)
  - `status`: Enum (Pending, Completed)
  - `order`: Integer (Sequence in the day)

- **Session**: Represents the current state.
  - `tasks`: List of Task objects
  - `mode`: Enum (Planning, Focus)

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Users can add 5 tasks and enter Focus Mode in under 30 seconds.
- **SC-002**: The persistent notification uses a compact layout, and the 'Focus Mode' screen is optimized for minimal visual clutter.
- **SC-003**: The app retains state 100% of the time after forced closure/restart.
- **SC-004**: Users complete the "Planning -> Focus -> Complete All" cycle without needing to resize or move the widget manually (default positioning works).
- **SC-005**: Animations run at 60fps on standard hardware to ensure the "juicy" feel is smooth.
