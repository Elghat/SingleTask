# Tasks: Game Loop Polish & Interactions

**Feature Branch**: `001-game-loop-polish`
**Status**: Pending
**Spec**: [specs/001-game-loop-polish/spec.md](../spec.md)
**Plan**: [specs/001-game-loop-polish/plan.md](../plan.md)

## Implementation Strategy
This feature focuses on UI polish and interaction logic.
1.  **Phase 1 (Setup)**: We must first ensure the Material Symbols font is available, as all UI tasks depend on it.
2.  **Phase 2 (Foundation)**: We verify the data model supports ordering to enable the "Defer" logic.
3.  **Phase 3 (US1)**: We implement the Planning Page improvements. This is P1 and visual.
4.  **Phase 4 (US2)**: We implement the Focus Page logic (Defer/Safety). This depends on the Foundation phase.

## Dependencies

- **Phase 1 & 2** must be completed before **Phase 3 & 4**.
- **Phase 3 (US1)** and **Phase 4 (US2)** can be executed in parallel after Phase 2 is done.

## Phase 1: Setup

**Goal**: Initialize missing resources (Fonts).

- [ ] T001 Copy `MaterialSymbolsRounded.ttf` (or similar) to `src/SingleTask/Resources/Fonts/`.
- [ ] T002 Register the new font alias `MaterialSymbols` in `src/SingleTask/MauiProgram.cs`.

## Phase 2: Foundation (Blocking)

**Goal**: Ensure data model supports ordering logic.

- [ ] T003 Verify `Order` property exists in `src/SingleTask.Core/Models/TaskItem.cs` and add `[Ignore]` `DisplayIndex` property for UI numbering.
- [ ] T004 Ensure `src/SingleTask.Core/Services/DatabaseService.cs` sorts results by `Order` in `GetTasksAsync()`.

## Phase 3: Planning Page Polish (US1)

**Goal**: Visual distinction for completed tasks and icon-based actions.
**Independent Test**: Run app, complete a task (check style), check Add/Start icons.

- [ ] T005 [US1] Update `src/SingleTask.Core/ViewModels/PlanningViewModel.cs` to populate `DisplayIndex` when loading tasks.
- [ ] T006 [US1] Update `src/SingleTask/Views/PlanningPage.xaml` to display `DisplayIndex` next to task title.
- [ ] T007 [US1] Update `src/SingleTask/Views/PlanningPage.xaml` `CollectionView` `DataTemplate` to apply `Opacity="0.5"` and `InputTransparent="True"` when `IsCompleted` is true.
- [ ] T008 [US1] Replace "Add" text button with `MaterialSymbols` ArrowUp icon in `src/SingleTask/Views/PlanningPage.xaml`.
- [ ] T009 [US1] Replace "Start" text button with `MaterialSymbols` Rocket icon in `src/SingleTask/Views/PlanningPage.xaml`.

## Phase 4: Focus Page Logic (US2)

**Goal**: Defer capability and Quit safety.
**Independent Test**: Enter Focus, Defer task (verify order change), try to close (verify alert).

- [ ] T010 [US2] Implement `DeferTaskAsync` logic (calculate Max Order + 1) in `src/SingleTask.Core/ViewModels/FocusViewModel.cs`.
- [ ] T011 [US2] Add `DeferCommand` exposing the logic in `src/SingleTask.Core/ViewModels/FocusViewModel.cs`.
- [ ] T012 [US2] Add "Defer to Later" button (LowPriority Icon) binding to `DeferCommand` in `src/SingleTask/Views/FocusPage.xaml`.
- [ ] T013 [US2] Update "Done" button style to be "Hero" (Large, Round/Prominent) in `src/SingleTask/Views/FocusPage.xaml` (See `Styles.xaml` for `RoundButtonStyle` or create new).
- [ ] T014 [US2] Implement back button interception with `DisplayAlert` confirmation in `src/SingleTask/Views/FocusPage.xaml.cs` (override `OnBackButtonPressed` or `Shell.BackButtonBehavior`).

## Phase 5: Polish & Review

- [ ] T015 Verify tab order and visual alignment of new icons on both pages.
- [ ] T016 Run `dotnet format` on modified files.
