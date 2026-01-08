# Tasks: UI Overhaul - "Warm Paper" Design System

**Input**: Design documents from `/specs/003-warm-paper-ui/`
**Prerequisites**: plan.md (required), spec.md (required)
**Feature Branch**: `003-warm-paper-ui`

**Tests**: ViewModels logic (Navigation) must be unit tested. UI visual fidelity requires manual verification against prototypes.

## Phase 1: Foundation (Theme & Fonts)

**Purpose**: Establish the visual language (Fonts & Colors) globally.

- [ ] T001 [Foundation] Add `Playfair Display` and `Inter` font files (or placeholders) to `src/SingleTask/Resources/Fonts/`
- [ ] T002 [Foundation] Register fonts in `MauiProgram.cs` with aliases `PlayfairDisplay` and `Inter`
- [ ] T003 [Foundation] Update `src/SingleTask/Resources/Styles/Colors.xaml` with "Warm Paper" palette (`#fdfbf7`, `#d97706`, etc.)
- [ ] T004 [Foundation] Update `src/SingleTask/Resources/Styles/Styles.xaml` to use new fonts and define `Headline`, `SubHeadline`, `CardSurfaceStyle`, `PrimaryButtonStyle`

**Checkpoint**: App runs with new background color and fonts.

---

## Phase 2: User Story 1 - Circular Navigation Logic

**Goal**: Implement strictly circular flow (Plan -> Focus -> Celebration -> Plan).
**Independent Test**: Unit tests verify `NavigationService` calls on specific Commands.

### Tests for User Story 1

- [ ] T005 [P] [US1] Create/Update `PlanningViewModelTests.cs`: Verify `SelectTaskCommand` navigates to `FocusPage` with Task parameter
- [ ] T006 [P] [US1] Create/Update `FocusViewModelTests.cs`: Verify completion navigates to `CelebrationPage` (instead of next task)
- [ ] T007 [P] [US1] Create `CelebrationViewModelTests.cs`: Verify `DismissCommand` navigates to `///PlanningPage` (Root)

### Implementation for User Story 1

- [ ] T008 [US1] Update `PlanningViewModel.cs`: Implement `SelectTaskCommand`
- [ ] T009 [US1] Update `FocusViewModel.cs`: Remove auto-advance logic, implement navigation to `CelebrationPage` on complete
- [ ] T010 [US1] Create `CelebrationViewModel.cs`: Implement `DismissCommand`
- [ ] T011 [US1] Register `CelebrationViewModel` and `CelebrationPage` in `MauiProgram.cs` and `AppShell.xaml.cs` routes

**Checkpoint**: Unit tests pass.

---

## Phase 3: User Story 2 - Planning Page Refactor

**Goal**: "Good Morning" Dashboard with **STRICT** ScrollView implementation.
**Independent Test**: Manual verification of scrolling on Android (No crash).

### Implementation for User Story 2

- [ ] T012 [US2] Refactor `PlanningPage.xaml`:
    - Remove `CollectionView`
    - Add `ScrollView` -> `VerticalStackLayout` -> `BindableLayout`
    - Add "Good Morning" Header (Headline Style)
    - Move Input to bottom row (Grid Row 2)
    - Apply `CardSurfaceStyle` to task items
    - Add `TapGestureRecognizer` to cards binding to `SelectTaskCommand`

**Checkpoint**: Dashboard matches `stitch_task_list/task_list` prototype.

---

## Phase 4: User Story 3 - Focus & Celebration UI

**Goal**: High-impact "Focus" mode and rewarding "Celebration".
**Independent Test**: Visual match with prototypes.

### Implementation for User Story 3

- [ ] T013 [US3] Refactor `FocusPage.xaml`:
    - Apply `LargeTitleLabelStyle` (Huge Typography)
    - Add Placeholder Timer visual
    - Make "DONE" button prominent (Round 80x80)
    - Remove old internal celebration overlays
- [ ] T014 [US3] Create `CelebrationPage.xaml`:
    - Implement "All caught up" visuals (Confetti, Message)
    - Add "Continue" button binding to `DismissCommand`
    - Create code-behind `CelebrationPage.xaml.cs` for Confetti animation (if keeping it)

**Checkpoint**: Focus and Celebration pages match prototypes.

---

## Phase 5: Verification & Polish

**Purpose**: Ensure stability constraints are met.

- [ ] T015 [Polish] Verify Android List Scrolling (Torture Test): ensure no crashes with many items
- [ ] T016 [Polish] Verify Font rendering on Android (Aliases working or real files needed)
- [ ] T017 [Polish] Check Navigation Stack depth (ensure `///PlanningPage` correctly resets stack)
