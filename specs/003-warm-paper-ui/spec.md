# Feature Specification: UI Overhaul - "Warm Paper" Design System

**Feature Branch**: `003-warm-paper-ui`  
**Created**: 2025-12-14  
**Status**: Draft  
**Input**: User description provided in chat.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Core Task Completion Loop (Priority: P1)

As a user, I want to select a task, focus on it, and celebrate its completion, so that I feel productive and rewarded.

**Why this priority**: This is the strict circular flow required for the application's core value proposition.

**Independent Test**: Can be fully tested by launching the app, clicking a task, marking it done, viewing the celebration, and returning to the list.

**Acceptance Scenarios**:

1. **Given** the `PlanningPage` (Dashboard), **When** I select a task card, **Then** the app navigates to the `FocusPage` with that task context.
2. **Given** the `FocusPage`, **When** I interact with the "Done" button, **Then** the app navigates to the `CelebrationPage`.
3. **Given** the `CelebrationPage`, **When** I dismiss the celebration (or interaction completes), **Then** the app returns to the `PlanningPage`.

---

### User Story 2 - Dashboard & "Warm Paper" Visuals (Priority: P1)

As a user, I want a visually distinct "Warm Paper" dashboard, so that the app feels welcoming and readable.

**Why this priority**: Establishes the new visual identity and landing experience.

**Independent Test**: Launch the app and verify colors, fonts, and layout match the "Stitch Task List" prototype.

**Acceptance Scenarios**:

1. **Given** the app launches, **Then** the background color is Warm Paper (`#fdfbf7`) and fonts are Playfair Display/Inter.
2. **Given** the `PlanningPage`, **Then** I see the header "Good Morning", a Progress Ring, and a scrollable list of tasks.
3. **Given** the task list, **When** I scroll, **Then** the motion is smooth and the layout is stable (no crashes on Android).

---

### User Story 3 - Focus Mode (Priority: P2)

As a user, I want a distraction-free focus screen with large typography, so that I can concentrate on the active task.

**Why this priority**: Essential for the "Single Task" focus methodology.

**Independent Test**: Navigate to a task and verify the UI elements against the "Focus Mode" prototype.

**Acceptance Scenarios**:

1. **Given** the `FocusPage`, **Then** the task title is displayed with huge typography.
2. **Given** the `FocusPage`, **Then** a Focus Timer and a prominent "Done" interaction are visible.

---

### Edge Cases

- **Task List Overflow**: What happens when there are many tasks? (System must scroll using `ScrollView`).
- **Font Availability**: Ensure custom fonts (Playfair Display, Inter) are loaded; fallback if missing?
- **Navigation Stack**: Ensure the "Back" button behavior is consistent with the circular flow (e.g., typically Back from Focus returns to Plan, but Back from Celebration might need to clear stack).

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST implement the "Warm Paper" global theme.
    - Background: `#fdfbf7`
    - Primary: `#d97706`
    - Ink: `#2d2a26`
    - Paper White: `#fffefc`
- **FR-002**: System MUST use specific fonts globally: `Playfair Display` (Serif) and `Inter` (Sans-Serif).
- **FR-003**: System MUST implement the `PlanningPage` (Dashboard) refactor matching `stitch_task_list/task_list/` design.
    - Key Elements: Header "Good Morning", Progress Ring, Task Cards.
- **FR-004**: System MUST implement the `FocusPage` refactor matching `stitch_task_list/focus_mode/` design.
- **FR-005**: System MUST create a NEW `CelebrationPage` matching `stitch_task_list/task_completion_celebration/` design.
    - Key Elements: "All caught up" message, Success visuals.
- **FR-006**: System MUST enforce strict circular navigation: Planning -> Focus -> Celebration -> Planning.
- **FR-007**: Bottom-pinned elements (Input bars) MUST use `Grid` row definitions (e.g., `*, Auto`) instead of fixed positioning.

### Technical Constraints (Critical)

- **TC-001**: For `PlanningPage` list rendering, strict prohibition on `CollectionView`. MUST use `ScrollView` containing a `VerticalStackLayout` with `BindableLayout.ItemsSource` to prevent Android rendering crashes.
- **TC-002**: HTML-to-XAML mapping rules must be followed:
    - HTML `div` with shadow -> MAUI `Border` with `<Shadow>`.
    - HTML `rounded-xl` -> MAUI `StrokeShape="RoundRectangle [Value]"`.
    - HTML Flexbox (`gap-4`) -> MAUI `Spacing="16"`.
    - Use exact HEX codes from HTML files.

### Key Entities *(include if feature involves data)*

- **Task**: The primary entity being displayed, focused on, and completed.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Visual Fidelity: All 3 pages (Planning, Focus, Celebration) visually match the provided HTML/PNG prototypes in `stitch_task_list/`.
- **SC-002**: Navigation Stability: Users can complete the Planning -> Focus -> Celebration -> Planning loop 5 times in a row without navigation errors or stack inconsistencies.
- **SC-003**: Android Stability: The task list on `PlanningPage` renders and scrolls without crashing on Android devices.
