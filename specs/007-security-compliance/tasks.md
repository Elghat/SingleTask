# Tasks: security-compliance

**Input**: Design documents from `specs/007-security-compliance/`
**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, quickstart.md

**Tests**: Tests are included for key logic (Encryption, Validation) as requested in the plan.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2)
- Include exact file paths in descriptions

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and package management

- [ ] T001 Remove `sqlite-net-pcl` and add `sqlite-net-sqlcipher` package in `src/SingleTask/SingleTask.csproj` and `src/SingleTask.Core/SingleTask.Core.csproj`
- [ ] T002 Verify project builds successfully after package swap (resolving any immediate namespace changes)

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before user stories

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [ ] T003 Create `IEncryptionService.cs` interface in `src/SingleTask.Core/Services/`
- [ ] T004 Implement `EncryptionService.cs` in `src/SingleTask.Core/Services/` using `Microsoft.Maui.Storage.SecureStorage` for key management
- [ ] T005 [P] Create `EncryptionServiceTests.cs` in `tests/SingleTask.UnitTests/Services/` to verify key generation/retrieval
- [ ] T006 Register `IEncryptionService` and implementation in `src/SingleTask/MauiProgram.cs`

**Checkpoint**: Foundation ready - Encryption service is available for injection.

---

## Phase 3: User Story 1 - Secure Data Storage (Priority: P1) 🎯 MVP

**Goal**: Encrypt local database and prevent insecure backups

**Independent Test**: Verify DB file is encrypted and cannot be opened without key; verify ADB backup is empty.

### Tests for User Story 1

- [ ] T007 [P] [US1] Create `DatabaseMigrationTests.cs` in `tests/SingleTask.UnitTests/Services/` to simulate plaintext-to-encrypted migration scenarios

### Implementation for User Story 1

- [ ] T008 [US1] Refactor `DatabaseService` constructor in `src/SingleTask.Core/Services/DatabaseService.cs` to accept `IEncryptionService`
- [ ] T009 [US1] Update `DatabaseService` `InitAsync` method to implement SQLCipher connection with key and data migration logic (Plaintext -> Encrypted)
- [ ] T010 [US1] Update `MauiProgram.cs` to inject `IEncryptionService` into `DatabaseService` factory
- [ ] T011 [US1] Update `src/SingleTask/Platforms/Android/AndroidManifest.xml` to set `android:allowBackup="false"`

**Checkpoint**: User Story 1 complete - Data is secure at rest and protected from backup extraction.

---

## Phase 4: User Story 2 - Application Hardening (Priority: P2)

**Goal**: Improve application resilience and release security

**Independent Test**: Verify input validation limits, R8 obfuscation in release build, and service isolation.

### Implementation for User Story 2

- [ ] T012 [P] [US2] Update `PlanningViewModel.cs` in `src/SingleTask.Core/ViewModels/` to add validation logic (Max Length 500) in `AddTaskAsync`
- [ ] T013 [P] [US2] Update `PlanningViewModel.cs` in `src/SingleTask.Core/ViewModels/` to add rate limiting (debounce) in `AddTaskAsync`
- [ ] T014 [P] [US2] Update `FocusSessionService.cs` in `src/SingleTask/Platforms/Android/Services/` to explicitly set `[Service(Exported = false, ...)]`
- [ ] T015 [US2] Update `SingleTask.csproj` to increase `SupportedOSPlatformVersion` (MinSDK) to 26.0 (Android 8.0)
- [ ] T016 [US2] Update `SingleTask.csproj` to add `Release` configuration group with R8 (`AndroidLinkMode`, `PublishTrimmed`, `AndroidEnableProguard`) and Keystore environment variables
- [ ] T017 [US2] Wrap debug logging in `src/SingleTask/Platforms/Android/Services/AndroidFocusService.cs` with `#if DEBUG` preprocessor directives

**Checkpoint**: User Story 2 complete - Application is hardened against tampering and DoS.

---

## Phase 5: Polish & Cross-Cutting Concerns

**Purpose**: Validation and final cleanup

- [ ] T018 Run manual security verification steps from `specs/007-security-compliance/quickstart.md`
- [ ] T019 Update `PRIVACY.md` to mention data encryption (if applicable, or just internal note)

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies.
- **Foundational (Phase 2)**: Depends on Setup. BLOCKS all user stories.
- **User Story 1 (P1)**: Depends on Foundational (needs `EncryptionService`).
- **User Story 2 (P2)**: Depends on Foundational (technically independent, but follows P1 priority).

### Parallel Opportunities

- T003, T005 (Interfaces & Tests) can run in parallel with T004 (Implementation).
- US2 tasks (T012-T017) are largely independent of each other and can be parallelized.
- US1 and US2 can run in parallel after Phase 2 is complete.

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1 & 2.
2. Complete Phase 3 (US1).
3. Validate Encryption.
4. Deploy MVP (Secure Storage).

### Full Feature

1. Complete MVP.
2. Complete Phase 4 (US2).
3. Validate Hardening.
4. Final Release.
