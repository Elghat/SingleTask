# Implementation Plan: Game Loop Polish & Interactions

**Branch**: `001-game-loop-polish` | **Date**: Tuesday, 16 December 2025 | **Spec**: [specs/001-game-loop-polish/spec.md](../spec.md)
**Input**: Feature specification from `specs/001-game-loop-polish/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/commands/plan.md` for the execution workflow.

## Summary

This feature aims to polish the "Game Loop" of the application by refining the Planning and Focus pages. Key improvements include visually distinguishing completed tasks (dimmed, unclickable), replacing text buttons with intuitive icons (Arrow Up, Rocket, Defer), and implementing a safety mechanism (confirmation dialog) for quitting focus mode. The technical approach involves updating the XAML UI for the Planning and Focus pages, implementing new `RelayCommand`s in the ViewModels, and ensuring database updates for deferring tasks maintain data integrity.

## Technical Context

<!--
  ACTION REQUIRED: Replace the content in this section with the technical details
  for the project. The structure here is presented in advisory capacity to guide
  the iteration process.
-->

**Language/Version**: C# 12 / .NET 10 (Preview/RC) [NEEDS CLARIFICATION: Confirm exact .NET 10 SDK version available in environment]
**Primary Dependencies**: MAUI (Microsoft.Maui.Controls), CommunityToolkit.Mvvm (for RelayCommand)
**Storage**: SQLite (via existing DatabaseService)
**Testing**: xUnit (SingleTask.UnitTests project exists)
**Target Platform**: Android, Windows (WinUI 3)
**Project Type**: Single Project (MAUI)
**Performance Goals**: UI interactions < 16ms (60fps), Database updates < 100ms
**Constraints**: strictly .NET 10 MAUI, MVVM pattern, FontImageSource for icons
**Scale/Scope**: < 5 modified files, targeted UI/Logic updates

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- [x] **I. Modular Design**: Changes are localized to `SingleTask` (UI) and `SingleTask.Core` (ViewModels), maintaining separation.
- [x] **II. Explicit Contracts**: Interaction between View and ViewModel via Commands; Database interaction via `IDatabaseService`.
- [x] **III. Test-Driven Quality**: Unit tests for `Defer` logic in ViewModels will be required.
- [x] **IV. Documentation First**: Spec and Plan created before code.
- [x] **V. Microsoft Ecosystem Alignment**: Uses MAUI, MVVM Toolkit, and standard C#.

## Project Structure

### Documentation (this feature)

```text
specs/001-game-loop-polish/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command) - Likely minimal/N/A
├── quickstart.md        # Phase 1 output (/speckit.plan command) - N/A
├── contracts/           # Phase 1 output (/speckit.plan command) - N/A
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (repository root)

```text
# Option 1: Single project (DEFAULT)
src/
├── SingleTask/
│   ├── Views/
│   │   ├── PlanningPage.xaml
│   │   ├── PlanningPage.xaml.cs
│   │   ├── FocusPage.xaml
│   │   └── FocusPage.xaml.cs
│   └── Resources/
│       └── Fonts/
└── SingleTask.Core/
    └── ViewModels/
        ├── PlanningViewModel.cs
        └── FocusViewModel.cs
```

**Structure Decision**: Standard MAUI MVVM structure. UI changes in `SingleTask/Views`, Business Logic in `SingleTask.Core/ViewModels`.

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| None | N/A | N/A |
