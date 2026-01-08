# Implementation Plan: MAUI Infrastructure Setup

**Branch**: `2-maui-infra-setup` | **Date**: 2025-12-11 | **Spec**: [specs/2-maui-infra-setup/spec.md](../spec.md)
**Input**: Feature specification from `/specs/2-maui-infra-setup/spec.md`

## Summary

Initialize a .NET 10 MAUI solution targeting Android and Windows. Configure the architecture with `CommunityToolkit.Mvvm` for state management and `sqlite-net-pcl` for local persistence. Establish a strict "Stable-only" NuGet policy and setup unit testing infrastructure.

## Technical Context

**Language/Version**: C# 13 / .NET 10 (Preview/RC if 10 not RTM, but Spec says SDK installed)
**Primary Dependencies**: 
- `CommunityToolkit.Mvvm` (Latest Stable)
- `sqlite-net-pcl` (Latest Stable)
- `SQLitePCLRaw.bundle_green` (Latest Stable)
**Storage**: SQLite (Local file)
**Testing**: xUnit, Moq (or NSubstitute)
**Target Platform**: Android (`net10.0-android`), Windows (`net10.0-windows10.0.19041.0`)
**Project Type**: Mobile/Desktop (MAUI)
**Performance Goals**: App startup < 5s (Android)
**Constraints**: Strict typing (nullable enabled), no preview packages, MVVM pattern.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- [x] **Modular Design**: Solution will separate App logic from Tests. Service interfaces defined.
- [x] **Explicit Contracts**: Services (INavigation, IDatabase) will be interface-based.
- [x] **Test-Driven Quality**: Unit test project included in initialization.
- [x] **Documentation First**: Plan & Spec exist.
- [x] **Microsoft Ecosystem**: .NET 10 MAUI, CommunityToolkit.
- [x] **Technical Constraints**: Strict nullable, stable packages enforced.

## Project Structure

### Documentation (this feature)

```text
specs/2-maui-infra-setup/
├── plan.md              # This file
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output (Minimal)
├── quickstart.md        # Phase 1 output
└── tasks.md             # Phase 2 output
```

### Source Code (repository root)

```text
src/
└── SingleTask/                 # Main MAUI Project
    ├── Models/                 # Data Entities
    ├── ViewModels/             # MVVM Logic
    ├── Views/                  # XAML Pages
    ├── Services/               # Core Services (Db, Nav)
    │   ├── Interfaces/         # Contracts
    │   └── Implementations/    # Concrete Logic
    ├── Resources/              # Assets
    └── MauiProgram.cs          # DI Setup

tests/
└── SingleTask.UnitTests/       # xUnit Project
    ├── ViewModels/
    └── Services/
```

**Structure Decision**: Standard MAUI Solution structure with a separate Unit Test project. `src` folder used to keep root clean.

## Complexity Tracking

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| None | N/A | N/A |
