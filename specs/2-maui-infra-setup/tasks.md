---

description: "Task list template for feature implementation"
---

# Tasks: MAUI Infrastructure Setup

**Input**: Design documents from `/specs/2-maui-infra-setup/`
**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md
**Feature Branch**: `2-maui-infra-setup`

**Tests**: Tests are included to verify the infrastructure setup as requested in the specification (User Stories have "Independent Test" criteria).

**Organization**: Tasks are grouped by user story (US1: Init, US2: MVVM, US3: DB) to enable independent implementation and testing.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel
- **[Story]**: US1, US2, US3

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Prepare the repository structure for the solution.

- [ ] T001 Create directory structure `src/SingleTask` and `tests/SingleTask.UnitTests` at repository root
- [ ] T002 [P] Create `global.json` to pin .NET SDK version (if specific version required, else skip)

---

## Phase 2: Foundational (User Story 1 - Solution Initialization)

**Purpose**: Initialize the solution and ensure it builds on target platforms (Blocking Prerequisite).
**Goal**: A compiling .NET 10 MAUI solution targeting Android and Windows.
**Independent Test**: `dotnet build` succeeds for both targets.

- [ ] T003 [US1] Run `dotnet new maui -n SingleTask` inside `src/` folder
- [ ] T004 [US1] Run `dotnet new xunit -n SingleTask.UnitTests` inside `tests/` folder
- [ ] T005 [US1] Create `SingleTask.sln` at root and add both projects using `dotnet sln`
- [ ] T006 [US1] Update `src/SingleTask/SingleTask.csproj` to strictly target `net10.0-android` and `net10.0-windows10.0.19041.0` (remove iOS/Mac if not needed per spec)
- [ ] T007 [US1] Enable `<Nullable>enable</Nullable>` in `src/SingleTask/SingleTask.csproj` if not default
- [ ] T008 [US1] Add project reference from `tests/SingleTask.UnitTests` to `src/SingleTask`
- [ ] T009 [US1] Verify build script or run manual build check for Android and Windows

**Checkpoint**: Solution exists and compiles.

---

## Phase 3: User Story 2 - MVVM Architecture Setup (Priority: P1)

**Goal**: Integrate `CommunityToolkit.Mvvm` and establish BaseViewModel.
**Independent Test**: Unit test verifies `INotifyPropertyChanged` behavior on BaseViewModel.

### Tests for User Story 2

- [ ] T010 [P] [US2] Create `BaseViewModelTests.cs` in `tests/SingleTask.UnitTests/ViewModels/` (initially failing or placeholder)

### Implementation for User Story 2

- [ ] T011 [US2] Add `CommunityToolkit.Mvvm` NuGet package to `src/SingleTask/SingleTask.csproj` (Stable version)
- [ ] T012 [P] [US2] Create `BaseViewModel.cs` inheriting `ObservableObject` in `src/SingleTask/ViewModels/`
- [ ] T013 [P] [US2] Create `MainViewModel.cs` inheriting `BaseViewModel` in `src/SingleTask/ViewModels/`
- [ ] T014 [US2] Register `MainViewModel` and `MainPage` in `src/SingleTask/MauiProgram.cs` for DI
- [ ] T015 [US2] Update `src/SingleTask/MainPage.xaml.cs` to inject `MainViewModel`
- [ ] T016 [US2] Implement unit test in `BaseViewModelTests.cs` to verify property notification

**Checkpoint**: MVVM pattern is established and verifiable.

---

## Phase 4: User Story 3 - Database Layer Configuration (Priority: P1)

**Goal**: Configure `sqlite-net-pcl` and database service.
**Independent Test**: App startup logic creates a DB file successfully.

### Tests for User Story 3

- [ ] T017 [P] [US3] Create `DatabaseServiceTests.cs` in `tests/SingleTask.UnitTests/Services/`

### Implementation for User Story 3

- [ ] T018 [US3] Add `sqlite-net-pcl` and `SQLitePCLRaw.bundle_green` NuGets to `src/SingleTask/SingleTask.csproj` (Stable versions)
- [ ] T019 [US3] Create `TestEntity.cs` in `src/SingleTask/Models/` for verification
- [ ] T020 [P] [US3] Create `IDatabaseService.cs` in `src/SingleTask/Services/Interfaces/`
- [ ] T021 [US3] Create `DatabaseService.cs` in `src/SingleTask/Services/Implementations/` using `FileSystem.AppDataDirectory`
- [ ] T022 [US3] Register `IDatabaseService` in `src/SingleTask/MauiProgram.cs`
- [ ] T023 [US3] Add simple connection check logic to `DatabaseService` (e.g. `Init()` method)

**Checkpoint**: Database service is ready for injection.

---

## Phase 5: Polish & Cross-Cutting Concerns

**Purpose**: Final verification.

- [ ] T024 Verify `quickstart.md` instructions against the actual implementation
- [ ] T025 Run `dotnet format` on the solution
- [ ] T026 Delete any unused default MAUI template files (e.g., WeatherForecast/Counter if irrelevant)

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies.
- **Foundational (Phase 2)**: Depends on Setup. Blocks all stories.
- **User Story 2 (MVVM)**: Depends on Foundational (Project existence).
- **User Story 3 (DB)**: Depends on Foundational (Project existence). Can run Parallel to US2.

### User Story Dependencies

- **US1 (Init)**: Blocks US2 and US3.
- **US2 (MVVM)**: Independent of US3.
- **US3 (DB)**: Independent of US2.

### Parallel Opportunities

- **US2 and US3 Implementation**: After Phase 2, Developer A can work on MVVM (US2) while Developer B works on Database (US3).
- **Test Creation**: Can happen in parallel with implementation within phases.

---

## Implementation Strategy

1. **Complete Setup & Foundational (US1)**: Get the empty app running.
2. **Parallel Branching**:
   - Branch `feature/mvvm-setup` for US2.
   - Branch `feature/db-setup` for US3.
3. **Merge**: Merge both into `2-maui-infra-setup`.
4. **Final Verification**: Run full suite.
