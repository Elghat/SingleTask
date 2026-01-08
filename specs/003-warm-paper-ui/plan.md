# Implementation Plan: UI Overhaul - "Warm Paper" Design System

**Branch**: `003-warm-paper-ui` | **Date**: 2025-12-14 | **Spec**: [specs/003-warm-paper-ui/spec.md](spec.md)
**Input**: Feature specification from `specs/003-warm-paper-ui/spec.md`

## Summary

Implement the "Warm Paper" design system, refactoring the application to follow a strict circular navigation flow (Plan -> Focus -> Celebration -> Plan). This involves replacing `CollectionView` with `ScrollView`+`BindableLayout` on the dashboard to resolve Android stability issues, establishing global styles (Colors/Fonts), and creating the new `CelebrationPage`.

## Technical Context

**Language/Version**: C# 11 (.NET 8/MAUI)
**Primary Dependencies**: 
- .NET MAUI (Built-in)
- `CommunityToolkit.Mvvm` (Existing)
**Storage**: N/A (UI only, data passed via Nav/Services)
**Testing**: xUnit (ViewModels), Manual Verification (UI/Navigation)
**Target Platform**: Android (primary stability target), Windows
**Performance Goals**: 
- Smooth scrolling on `PlanningPage` (Android)
- Zero navigation stack leaks in circular flow
**Constraints**: 
- **STRICT**: No `CollectionView` on `PlanningPage`.
- **STRICT**: Exact HEX match for "Warm Paper" palette.
- **STRICT**: Circular Navigation (PopToRoot behavior).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- [x] **Modular Design**: ViewModels separated from Views.
- [x] **Explicit Contracts**: Navigation via `INavigationService`.
- [x] **Test-Driven Quality**: ViewModel logic (Navigation commands) will be unit tested.
- [x] **Technical Constraints**: `ScrollView` constraint explicitly planned.
- [x] **User Value**: High-fidelity UI implementation ("Warm Paper").

## Project Structure

### Documentation (this feature)

```text
specs/003-warm-paper-ui/
├── plan.md              # This file
├── research.md          # Technical analysis
├── data-model.md        # N/A (UI focused)
└── tasks.md             # Phase 2 output (Test-Driven)
```

### Source Code (repository root)

```text
src/SingleTask/
├── Resources/
│   ├── Fonts/          # Playfair Display & Inter
│   └── Styles/
│       ├── Colors.xaml # Warm Paper Palette
│       └── Styles.xaml # Global Control Styles
├── Views/
│   ├── PlanningPage.xaml  # Refactor (ScrollView)
│   ├── FocusPage.xaml     # Refactor (Typography)
│   └── CelebrationPage.xaml # New
└── ViewModels/
    ├── PlanningViewModel.cs # Nav Logic Update
    ├── FocusViewModel.cs    # Nav Logic Update
    └── CelebrationViewModel.cs # New
```

## Complexity Tracking

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| Manual List Rendering | Android rendering crash with `CollectionView` | Native virtualization is unstable with complex borders/shadows in current MAUI version. |