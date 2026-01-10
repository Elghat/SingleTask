# Implementation Plan: Project Optimization Overhaul

**Branch**: `008-project-optimization` | **Date**: 2026-01-10 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/008-project-optimization/spec.md`

## Summary

This plan implements comprehensive project optimizations for the SingleTask .NET MAUI application, addressing critical stability issues (async void patterns), performance bottlenecks (database batching, list virtualization), build configuration improvements (AOT, trimming), and code quality enhancements. The primary technical approach leverages existing .NET 10 SDK capabilities without introducing new dependencies.

## Technical Context

**Language/Version**: C# 13 / .NET 10.0  
**Primary Dependencies**: 
- Microsoft.Maui.Controls (SDK-managed)
- CommunityToolkit.Mvvm 8.4.0
- sqlite-net-sqlcipher 1.9.172

**Storage**: SQLite with SQLCipher encryption (local file-based)  
**Testing**: xUnit 2.9.3 + Moq 4.20.72  
**Target Platform**: Android API 35 (min SDK 26), Windows 10.0.19041+  
**Project Type**: Mobile (.NET MAUI single project with Core library)  
**Performance Goals**: 
- Task reorder < 200ms (20+ items)
- 60fps scroll (50+ items)
- Cold startup < 2 seconds
  
**Constraints**: 
- No new dependencies
- Maintain backward compatibility
- No breaking changes to user data

**Scale/Scope**: Single-user local-first app, ~2,200 LOC, 4 pages

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Gate | Status | Notes |
|------|--------|-------|
| No new projects/libraries | ✅ PASS | All changes in existing `SingleTask` and `SingleTask.Core` projects |
| Test-first approach | ⚠️ ADVISORY | Existing tests cover ViewModels; new tests required for batch save |
| No breaking changes | ✅ PASS | Internal refactors only; public API unchanged |
| Simplicity/YAGNI | ✅ PASS | All changes justified by documented performance/stability issues |

**Pre-Phase 0 Result**: ✅ PASS - Proceed to research phase

## Project Structure

### Documentation (this feature)

```text
specs/008-project-optimization/
├── plan.md              # This file
├── research.md          # Phase 0 output - technology decisions
├── data-model.md        # Phase 1 output - entity changes
├── quickstart.md        # Phase 1 output - implementation guide
├── checklists/          # Quality validation
│   └── requirements.md
└── tasks.md             # Phase 2 output (created by /speckit.tasks)
```

### Source Code (repository root)

```text
src/
├── SingleTask/                        # MAUI Head Project
│   ├── Views/
│   │   ├── PlanningPage.xaml         # FR-005: BindableLayout → CollectionView
│   │   ├── PlanningPage.xaml.cs      # FR-001: async void OnAppearing fix
│   │   ├── CelebrationPage.xaml.cs   # FR-001, FR-003: async void + logging
│   │   └── FocusPage.xaml.cs         # No changes needed
│   ├── Services/
│   │   └── HapticService.cs          # FR-003: Add exception logging
│   ├── Platforms/Android/
│   │   └── Services/
│   │       ├── AudioService.cs       # FR-002, FR-015: Task.Delay + dispose
│   │       ├── FocusSessionService.cs # FR-016: Keep (primary service)
│   │       └── FocusForegroundService.cs # FR-016: Remove (duplicate)
│   ├── Resources/
│   │   └── Fonts/
│   │       └── [Remove OpenSans-*.ttf] # FR-010
│   └── SingleTask.csproj             # FR-006 to FR-009: Build config
│
├── SingleTask.Core/                   # Shared Business Logic
│   ├── ViewModels/
│   │   ├── FocusViewModel.cs         # FR-001: async void → async Task
│   │   └── MainViewModel.cs          # FR-011, FR-018: Remove or standardize
│   ├── Services/
│   │   ├── IDatabaseService.cs       # FR-004: Add SaveTasksAsync
│   │   └── DatabaseService.cs        # FR-004, FR-014: Batch + IAsyncDisposable
│   └── Models/
│       └── TestEntity.cs             # FR-011: Move to test project
│
tests/
└── SingleTask.UnitTests/
    └── Services/
        └── DatabaseServiceTests.cs   # New: Batch save tests

[Files to remove - FR-011, FR-012]
- src/SingleTask/Views/MainPage.xaml
- src/SingleTask/Views/MainPage.xaml.cs
- src/SingleTask/build_debug.log
- src/SingleTask/build_error_3.log
- src/SingleTask/preprocessed.xml
```

**Structure Decision**: Existing Mobile (Option 3) structure maintained. No new projects or structural changes required. All modifications are within existing `SingleTask` and `SingleTask.Core` projects.

## Complexity Tracking

> No constitution violations to justify. All changes are simplifications or bug fixes.

## Phase Summary

### Phase 0: Research (Completed in this plan)
- No external unknowns - all technologies already in use
- Best practices documented in research.md

### Phase 1: Design  
- Data model changes: `IDatabaseService` interface extension
- No API contracts needed (local-only app)
- Agent context updated

### Implementation Phases (for /speckit.tasks)

| Phase | Priority | Requirements | Effort |
|-------|----------|--------------|--------|
| 1 | P1-Critical | FR-001, FR-002 (async fixes) | 2h |
| 2 | P1-Critical | FR-004, FR-005 (batch + virtualization) | 4h |
| 3 | P2-High | FR-003, FR-014, FR-015 (logging, dispose) | 2h |
| 4 | P2-High | FR-006 to FR-009 (build config) | 1h |
| 5 | P2-Medium | FR-010 to FR-013 (resource cleanup) | 2h |
| 6 | P3-Low | FR-016 to FR-018 (consolidation) | 2h |

**Estimated Total**: ~13 hours of development work

## Generated Artifacts

See companion files:
- [research.md](./research.md) - Technology decisions and patterns
- [data-model.md](./data-model.md) - Entity and interface changes
- [quickstart.md](./quickstart.md) - Implementation guide
