# Tasks: Project Optimization Overhaul

**Input**: Design documents from `/specs/008-project-optimization/`  
**Prerequisites**: plan.md ✓, spec.md ✓, research.md ✓, data-model.md ✓, quickstart.md ✓

**Tests**: Tests are NOT explicitly requested. Unit test updates only where existing tests break.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2)
- Include exact file paths in descriptions

## Path Conventions

- **MAUI Project**: `src/SingleTask/` (head project)
- **Core Library**: `src/SingleTask.Core/` (business logic)
- **Tests**: `tests/SingleTask.UnitTests/`

---

## Phase 1: Setup (Baseline Capture)

**Purpose**: Capture baseline metrics and ensure clean starting point

- [ ] T001 Verify all existing unit tests pass by running `dotnet test tests/SingleTask.UnitTests`
- [ ] T002 Build Release APK and record baseline size: `dotnet publish -f net10.0-android -c Release`
- [ ] T003 Record baseline cold startup time on test device (manual measurement)
- [ ] T004 Create 20 test tasks and record task reorder latency (manual measurement)

---

## Phase 2: Foundational (Interface Changes)

**Purpose**: Core interface and service changes that MUST be complete before user story implementations

**⚠️ CRITICAL**: These changes affect multiple user stories

- [ ] T005 Add `SaveTasksAsync(IEnumerable<TaskItem> tasks)` method signature to `src/SingleTask.Core/Services/IDatabaseService.cs`
- [ ] T006 Add `IAsyncDisposable` to `IDatabaseService` interface declaration in `src/SingleTask.Core/Services/IDatabaseService.cs`
- [ ] T007 Implement `SaveTasksAsync` with `RunInTransactionAsync` in `src/SingleTask.Core/Services/DatabaseService.cs`
- [ ] T008 Implement `DisposeAsync()` method in `src/SingleTask.Core/Services/DatabaseService.cs`

**Checkpoint**: Interface changes complete - user story implementation can proceed

---

## Phase 3: User Story 1 - Stable Application Without Crash (Priority: P1) 🎯 MVP

**Goal**: Fix async void patterns that cause unhandled exceptions and app crashes

**Independent Test**: Trigger error conditions in FocusViewModel and verify graceful handling

### Implementation for User Story 1

- [ ] T009 [US1] Convert `async void Initialize` to `public async Task InitializeAsync` in `src/SingleTask.Core/ViewModels/FocusViewModel.cs` (line 78)
- [ ] T010 [US1] Add try-catch with debug logging to `InitializeAsync` method in `src/SingleTask.Core/ViewModels/FocusViewModel.cs`
- [ ] T011 [US1] Update constructor call from `Initialize()` to `_ = InitializeAsync()` in `src/SingleTask.Core/ViewModels/FocusViewModel.cs` (line ~52)
- [ ] T012 [US1] Add try-catch wrapper to `OnAppearing` in `src/SingleTask/Views/PlanningPage.xaml.cs` (already has, verify exception propagation)
- [ ] T013 [US1] Add try-catch wrapper to `OnAppearing` in `src/SingleTask/Views/CelebrationPage.xaml.cs` (line 13)
- [ ] T014 [US1] Update FocusViewModel unit tests to test new `InitializeAsync` signature in `tests/SingleTask.UnitTests/ViewModels/FocusViewModelTests.cs`

**Checkpoint**: App no longer crashes from async void exceptions

---

## Phase 4: User Story 2 - Fast Task Reordering (Priority: P1) 🎯 MVP

**Goal**: Task reordering completes in <200ms for 20+ tasks

**Independent Test**: Create 20 tasks, reorder them, verify instant response

### Implementation for User Story 2

- [ ] T015 [US2] Update `SaveTaskOrderAsync` to use batch `SaveTasksAsync` in `src/SingleTask.Core/ViewModels/PlanningViewModel.cs` (lines 239-247)
- [ ] T016 [US2] Add unit test for `SaveTasksAsync` batch operation in `tests/SingleTask.UnitTests/Services/DatabaseServiceTests.cs` (new file)
- [ ] T017 [US2] Verify reorder debounce (SEC-012) still works with batch save in `src/SingleTask.Core/ViewModels/PlanningViewModel.cs`

**Checkpoint**: Task reordering is instant (<200ms)

---

## Phase 5: User Story 3 - Smooth Scrolling Task List (Priority: P2)

**Goal**: Task list scrolls at 60fps with 50+ items

**Independent Test**: Add 50 tasks, scroll rapidly, observe smooth animation

### Implementation for User Story 3

- [ ] T018 [US3] Replace `ScrollView > VerticalStackLayout > BindableLayout` with `CollectionView` in `src/SingleTask/Views/PlanningPage.xaml` (lines 32-164)
- [ ] T019 [US3] Add `LinearItemsLayout` with `ItemSpacing="12"` to CollectionView in `src/SingleTask/Views/PlanningPage.xaml`
- [ ] T020 [US3] Set `SelectionMode="None"` on CollectionView in `src/SingleTask/Views/PlanningPage.xaml`
- [ ] T021 [US3] Verify DataTemplate bindings work correctly within CollectionView (manual test)
- [ ] T022 [US3] Verify reorder button commands still bind to ViewModel in CollectionView template

**Checkpoint**: Scrolling is smooth at 60fps

---

## Phase 6: User Story 4 - Reduced APK Size (Priority: P2)

**Goal**: Release APK size reduced by at least 30%

**Independent Test**: Compare APK size before/after optimization

### Implementation for User Story 4

- [ ] T023 [P] [US4] Delete unused font file `src/SingleTask/Resources/Fonts/OpenSans-Regular.ttf`
- [ ] T024 [P] [US4] Delete unused font file `src/SingleTask/Resources/Fonts/OpenSans-Semibold.ttf`
- [ ] T025 [P] [US4] Delete unused page `src/SingleTask/Views/MainPage.xaml`
- [ ] T026 [P] [US4] Delete unused code-behind `src/SingleTask/Views/MainPage.xaml.cs`
- [ ] T027 [P] [US4] Delete unused ViewModel `src/SingleTask.Core/ViewModels/MainViewModel.cs`
- [ ] T028 [US4] Remove MainPage/MainViewModel DI registrations from `src/SingleTask/MauiProgram.cs` (lines 28-29)
- [ ] T029 [US4] Verify build succeeds after file deletions with `dotnet build src/SingleTask`

**Checkpoint**: APK size reduced (verify against baseline)

---

## Phase 7: User Story 5 - Faster App Cold Startup (Priority: P2)

**Goal**: Cold startup time reduced by 200ms+

**Independent Test**: Time app launch to interactive state on test device

### Implementation for User Story 5

- [ ] T030 [US5] Add `<RunAOTCompilation>true</RunAOTCompilation>` to Release PropertyGroup in `src/SingleTask/SingleTask.csproj`
- [ ] T031 [US5] Add `<TrimMode>full</TrimMode>` to Release PropertyGroup in `src/SingleTask/SingleTask.csproj`
- [ ] T032 [US5] Add `<AndroidEnableMarshalMethods>true</AndroidEnableMarshalMethods>` to Release PropertyGroup in `src/SingleTask/SingleTask.csproj`
- [ ] T033 [US5] Add `<AndroidEnablePreloadAssemblies>true</AndroidEnablePreloadAssemblies>` to Release PropertyGroup in `src/SingleTask/SingleTask.csproj`
- [ ] T034 [US5] Build Release and verify no trimming errors with `dotnet publish -f net10.0-android -c Release`

**Checkpoint**: Build succeeds with new optimizations

---

## Phase 8: User Story 6 - Reliable Audio Playback (Priority: P2)

**Goal**: Celebration sound plays without UI freeze

**Independent Test**: Complete all tasks, observe UI responsiveness during sound

### Implementation for User Story 6

- [ ] T035 [US6] Replace `Thread.Sleep(250)` with `await Task.Delay(250)` in `src/SingleTask/Platforms/Android/Services/AudioService.cs` (lines 38, 40)
- [ ] T036 [US6] Change method to async: remove `Task.Run` wrapper in `src/SingleTask/Platforms/Android/Services/AudioService.cs`
- [ ] T037 [US6] Add `toneGen.Release()` after tone playback in `src/SingleTask/Platforms/Android/Services/AudioService.cs`
- [ ] T038 [US6] Verify celebration animation remains responsive (manual test)

**Checkpoint**: Audio plays without blocking UI

---

## Phase 9: User Story 7 - Debuggable Production Issues (Priority: P3)

**Goal**: All exceptions are logged in debug builds

**Independent Test**: Code review confirms all catch blocks have logging

### Implementation for User Story 7

- [ ] T039 [P] [US7] Add debug logging to catch block at line 78 in `src/SingleTask.Core/Services/DatabaseService.cs`
- [ ] T040 [P] [US7] Add debug logging to catch block at line 101 in `src/SingleTask.Core/Services/DatabaseService.cs`
- [ ] T041 [P] [US7] Add debug logging to catch block at line 130 in `src/SingleTask.Core/Services/DatabaseService.cs`
- [ ] T042 [P] [US7] Add debug logging to catch blocks in `src/SingleTask/Services/HapticService.cs` (lines 13, 26)
- [ ] T043 [P] [US7] Add debug logging to catch block in `src/SingleTask/Views/CelebrationPage.xaml.cs` (line 27)
- [ ] T044 [P] [US7] Add debug logging to catch blocks in `src/SingleTask/Platforms/Android/Services/AudioService.cs` (lines 17, 43)

**Checkpoint**: All catch blocks have appropriate logging

---

## Phase 10: User Story 8 - Clean Codebase (Priority: P3)

**Goal**: Remove dead code and orphaned files

**Independent Test**: Solution builds clean, no unused files in src/

### Implementation for User Story 8

- [ ] T045 [P] [US8] Delete or move `TestEntity.cs` from `src/SingleTask.Core/Models/TestEntity.cs` to test project if needed
- [ ] T046 [P] [US8] Delete duplicate service file `src/SingleTask/Platforms/Android/Services/FocusForegroundService.cs`
- [ ] T047 [P] [US8] Delete build artifact `src/SingleTask/build_debug.log`
- [ ] T048 [P] [US8] Delete build artifact `src/SingleTask/build_error_3.log`
- [ ] T049 [P] [US8] Delete build artifact `src/SingleTask/preprocessed.xml`
- [ ] T050 [US8] Add `*.log` and `preprocessed.xml` to `.gitignore`
- [ ] T051 [US8] Remove `TestEntity` references from `DatabaseService.cs` if entity removed
- [ ] T052 [US8] Remove `TestEntity` references from `IDatabaseService.cs` if entity removed

**Checkpoint**: Codebase is clean with no orphaned files

---

## Phase 11: Polish & Validation

**Purpose**: Final validation and cross-cutting concerns

- [ ] T053 Run all unit tests to verify no regressions: `dotnet test tests/SingleTask.UnitTests`
- [ ] T054 Build Release APK and compare to baseline size
- [ ] T055 Measure cold startup time and compare to baseline
- [ ] T056 Test task reordering with 20+ tasks and verify <200ms
- [ ] T057 Test scrolling with 50+ tasks and verify 60fps
- [ ] T058 Complete full user flow (add tasks → focus → complete → celebrate)
- [ ] T059 Update `docs/PROJECT_OPTIMIZATION_REPORT.md` with actual results
- [ ] T060 Commit all changes with message: "feat(optimization): implement project optimization overhaul #008"

---

## Dependencies & Execution Order

### Phase Dependencies

```
Phase 1: Setup         → No dependencies (capture baselines)
Phase 2: Foundational  → Depends on Phase 1 (interface changes)
Phase 3-10: User Stories → All depend on Phase 2 completion
Phase 11: Polish       → Depends on all desired user stories
```

### User Story Dependencies

| Story | Depends On | Can Parallel With |
|-------|-----------|-------------------|
| US1 (Async Void) | Phase 2 | US2, US4, US5, US6, US7, US8 |
| US2 (Batch Save) | Phase 2, T005-T008 | US1, US4, US5, US6, US7, US8 |
| US3 (CollectionView) | Phase 2 | US1, US2, US4, US5, US6, US7, US8 |
| US4 (APK Size) | Phase 2 | US1, US2, US3, US5, US6, US7, US8 |
| US5 (Startup) | Phase 2 | US1, US2, US3, US4, US6, US7, US8 |
| US6 (Audio) | Phase 2 | US1, US2, US3, US4, US5, US7, US8 |
| US7 (Logging) | Phase 2 | US1, US2, US3, US4, US5, US6, US8 |
| US8 (Cleanup) | Phase 2, US4 (shares file deletions) | US1, US2, US3, US5, US6, US7 |

### Critical Path

```
T001-T004 (Baseline) → T005-T008 (Interfaces) → T009-T014 (US1) → T053-T060 (Validation)
```

**Minimum MVP**: Phases 1, 2, 3, 4, 11 = ~15 tasks

---

## Parallel Opportunities

### Setup Phase (P1)
```
All tasks sequential (baseline capture)
```

### Foundational Phase (P2)
```
T005 + T006 can run in parallel (interface file, different methods)
T007 + T008 can run in parallel (implementation, different methods)
```

### User Story Phases (P3+)
```
All user stories can run in parallel after Foundational completes:
- Developer A: US1 + US2 (critical path)
- Developer B: US4 + US8 (file cleanup)
- Developer C: US5 + US7 (build config + logging)
- Developer D: US3 + US6 (UI + audio)
```

### Within US4 (File Deletions)
```
T023 + T024 + T025 + T026 + T027 can all run in parallel (different files)
```

### Within US7 (Logging)
```
T039 + T040 + T041 + T042 + T043 + T044 can all run in parallel (different files)
```

---

## Implementation Strategy

### MVP First (US1 + US2 Only)

1. Complete Phase 1: Setup (T001-T004)
2. Complete Phase 2: Foundational (T005-T008)
3. Complete Phase 3: User Story 1 (T009-T014)
4. Complete Phase 4: User Story 2 (T015-T017)
5. **STOP and VALIDATE**: Test critical stability and performance
6. Proceed to remaining stories if MVP validated

### Incremental Delivery

```
Setup → Foundational → US1 (async fix) → US2 (batch) → VALIDATE
                     → US3 (scroll) → US4 (size) → US5 (startup) → VALIDATE
                     → US6 (audio) → US7 (logging) → US8 (cleanup) → VALIDATE
                     → Polish → DONE
```

### Estimated Effort

| Phase | Tasks | Effort |
|-------|-------|--------|
| Setup | 4 | 30 min |
| Foundational | 4 | 1 hr |
| US1 (Async) | 6 | 2 hr |
| US2 (Batch) | 3 | 1 hr |
| US3 (Scroll) | 5 | 2 hr |
| US4 (Size) | 7 | 1 hr |
| US5 (Startup) | 5 | 30 min |
| US6 (Audio) | 4 | 1 hr |
| US7 (Logging) | 6 | 1 hr |
| US8 (Cleanup) | 8 | 1 hr |
| Polish | 8 | 2 hr |
| **Total** | **60** | **~13 hr** |

---

## Notes

- [P] tasks = different files, no dependencies
- [Story] label maps task to specific user story for traceability
- Each user story is independently completable and testable
- Commit after each task or logical group
- Stop at any checkpoint to validate story independently
- T005-T008 are BLOCKING - no user story work until complete
