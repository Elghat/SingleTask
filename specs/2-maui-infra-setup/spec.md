# Feature Specification: MAUI Infrastructure Setup

**Feature Branch**: `2-maui-infra-setup`  
**Created**: 2025-12-11  
**Status**: Draft  
**Input**: User description: "**SYSTEM CONSTRAINTS (CRITICAL):** * **Framework:** .NET 10 MAUI (Target framework: `net10.0-android`). * **Environment:** I have the .NET 10 SDK installed. Do not downgrade to .NET 8 unless strictly necessary. * **Stability Rule:** Use ONLY **Stable** versions of NuGet packages. DO NOT use "-preview", "-rc", or "-beta" versions of libraries, even if they match .NET 10. * **Architecture:** MVVM with `CommunityToolkit.Mvvm` (Latest Stable). * **Database:** `sqlite-net-pcl` (Latest Stable). * **Target:** Android Emulator & Windows Machine."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Solution Initialization (Priority: P1)

A developer initializes the repository with the correct .NET 10 MAUI solution structure, ensuring all project settings match the mandated constraints.

**Why this priority**: Blocking prerequisite for all other development.

**Independent Test**: Can be tested by running `dotnet build` and `dotnet run` on the target platforms immediately after initialization.

**Acceptance Scenarios**:

1. **Given** the .NET 10 SDK is installed, **When** the solution is built, **Then** it compiles with zero errors and zero warnings targeting `net10.0-android` and `net10.0-windows10...`.
2. **Given** the solution is initialized, **When** deployed to an Android Emulator, **Then** the application launches successfully.
3. **Given** the solution is initialized, **When** deployed to a Windows Machine, **Then** the application launches successfully.

---

### User Story 2 - MVVM Architecture Setup (Priority: P1)

The developer integrates `CommunityToolkit.Mvvm` and establishes the base classes (ViewModelBase, etc.) to enforce the MVVM pattern across the application.

**Why this priority**: Establishes the architectural pattern early to prevent spaghetti code in the code-behind.

**Independent Test**: Can be tested by creating a simple "Hello World" ViewModel bound to a View and verifying command binding works.

**Acceptance Scenarios**:

1. **Given** a ViewModel inheriting from the base class, **When** a property changes, **Then** the `INotifyPropertyChanged` event is fired (verified via unit test or UI binding).
2. **Given** the project dependencies, **When** inspecting `csproj`, **Then** `CommunityToolkit.Mvvm` is present and is a **Stable** version (no -pre, -rc).

---

### User Story 3 - Database Layer Configuration (Priority: P1)

The developer configures `sqlite-net-pcl` and establishes a robust database initialization routine that works on both Android and Windows.

**Why this priority**: Data persistence is a core requirement for the main application (SingleTask).

**Independent Test**: Can be tested by running a startup routine that creates a dummy table and inserts a record.

**Acceptance Scenarios**:

1. **Given** the app launches, **When** the database service initializes, **Then** a valid SQLite connection is established.
2. **Given** a simple data entity, **When** the app attempts to write to the DB, **Then** the data is persisted without exception.
3. **Given** project dependencies, **When** inspecting `csproj`, **Then** `sqlite-net-pcl` is present and is a **Stable** version.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST be initialized as a .NET 10 MAUI solution.
- **FR-002**: System MUST target `net10.0-android` and Windows (WinUI 3).
- **FR-003**: System MUST NOT use any preview, RC, or beta NuGet packages; ALL dependencies MUST be Stable.
- **FR-004**: System MUST utilize `CommunityToolkit.Mvvm` for the Model-View-ViewModel pattern.
- **FR-005**: System MUST utilize `sqlite-net-pcl` for local data persistence.
- **FR-006**: System MUST provide a mechanism to detect and handle platform-specific file paths for the SQLite database.

### Key Entities

- **BaseViewModel**: Core class implementing `ObservableObject` from the toolkit.
- **DatabaseConnection**: Wrapper or service managing the `SQLiteAsyncConnection`.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Application build time (clean build) is under 60 seconds on standard dev hardware.
- **SC-002**: Application startup time on Android Emulator is under 5 seconds (cold start).
- **SC-003**: 100% of NuGet packages are explicitly pinned to Stable versions.
- **SC-004**: Code analysis reports 0 warnings related to architecture or platform compatibility.
