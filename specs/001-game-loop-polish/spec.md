# Feature Specification: Game Loop Polish & Interactions

**Feature Branch**: `001-game-loop-polish`
**Created**: Tuesday, 16 December 2025
**Status**: Draft
**Input**: User description provided in CLI.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Planning Page Polish (Priority: P1)

As a user, I want the planning page to clearly distinguish between active and completed tasks, and provide intuitive icons for actions, so that I can easily manage my day.

**Why this priority**: Core to the user experience; the current raw interactions feel unfinished.

**Independent Test**: Can be tested by adding tasks, marking them complete, and verifying visual/interactive states.

**Acceptance Scenarios**:

1. **Given** a list of tasks, **When** I view a completed task, **Then** it must be visually dimmed (reduced opacity) and have strikethrough text.
2. **Given** a completed task, **When** I attempt to tap it, **Then** nothing should happen (it is strictly unclickable).
3. **Given** the planning page, **When** I look for the "Add" action, **Then** I see an "Arrow Up" icon button instead of text.
4. **Given** the planning page, **When** I look for the "Start" action, **Then** I see a "Rocket" icon button instead of text.
5. **Given** the task list, **When** displayed, **Then** each task shows a sequential number (1, 2, 3...) indicating its order.

---

### User Story 2 - Focus Page Logic & Safety (Priority: P1)

As a user in focus mode, I want options to defer tasks and protection against accidental quitting, so that I maintain flow and don't lose progress.

**Why this priority**: Prevents data loss and adds critical flexibility to the workflow.

**Independent Test**: Can be tested by entering focus mode, deferring a task, and attempting to close the page.

**Acceptance Scenarios**:

1. **Given** I am in Focus Mode, **When** I realize I cannot do the task now, **Then** I can tap a "Defer to Later" button (low_priority icon).
2. **Given** I tap "Defer to Later", **When** the action executes, **Then** the task moves to the end of the list and I return to the Planning Page.
3. **Given** I am in Focus Mode, **When** I click the "X" (Close) button, **Then** a native confirmation alert appears ("Stop Focus? Progress will be lost.").
4. **Given** the confirmation alert, **When** I confirm, **Then** the focus session ends; **When** I cancel, **Then** I stay in focus mode.
5. **Given** I complete a task, **When** viewing the "Done" button, **Then** it appears as a large "Hero" element styled with the Warm Paper theme.

### Edge Cases

- **Deferring Last/Only Item**: If the user defers the only task or the last task, it remains in the list (effectively no change in order) but navigates back to Planning Page.
- **Rapid Navigation**: If user clicks "Defer" or "Done" multiple times rapidly, the system should handle it gracefully (debounce) to prevent navigation errors.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The system MUST render completed tasks on the Planning Page with reduced opacity and strikethrough text decoration.
- **FR-002**: The system MUST prevent all user interaction (taps, gestures) with completed tasks in the Planning Page.
- **FR-003**: The system MUST display an "Arrow Up" icon for the "Add Task" button.
- **FR-004**: The system MUST display a "Rocket" icon for the "Start Focus" button.
- **FR-005**: The system MUST display a visible 1-based index (1, 2, 3...) next to each task in the Planning Page list.
- **FR-006**: The Focus Page MUST include a "Defer to Later" button with a `low_priority` (or equivalent) icon.
- **FR-007**: The Defer action MUST update the task's order to be last in the pending list.
- **FR-008**: The Defer action MUST navigate the user back to the Planning Page immediately.
- **FR-009**: The Focus Page "Close" ("X") button MUST trigger a native confirmation dialog warning of progress loss before exiting.
- **FR-010**: The "Done" button on the Focus Page MUST be styled as a prominent "Hero" button consistent with the Warm Paper design language.

### Technical Constraints

- **TC-001**: All new interactions MUST be implemented using MVVM pattern and `RelayCommand`.
- **TC-002**: Icons MUST use `FontImageSource` referencing the existing Material Symbols font setup.
- **TC-003**: Database updates for "Defer" (OrderIndex) MUST ensure data consistency.
- **TC-004**: Visual styles (Opacity 0.5) must be applied via XAML styles or triggers.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Completed tasks have zero interactive elements (cannot be selected/tapped).
- **SC-002**: Defer action successfully reorders the task to the bottom of the list 100% of the time.
- **SC-003**: Accidental quits from Focus Page are prevented by the confirmation dialog (User must explicitly confirm to quit).
- **SC-004**: UI elements (Icons, Hero Button) render correctly across supported platforms (Android/Windows) without layout regressions.