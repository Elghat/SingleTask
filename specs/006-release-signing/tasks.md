# Tasks: Release Signing (Keystore & AAB)

**Feature**: `006-release-signing`
**Status**: Pending
**Input**: Plan from `specs/006-release-signing/plan.md`

## Phase 1: Setup
**Goal**: Ensure environment is ready for key generation and signing.

- [ ] T001 Verify JDK and keytool availability via terminal command

## Phase 2: Foundational (Blocking)
**Goal**: Secure the repository before generating sensitive credentials.

- [ ] T002 Update .gitignore to exclude *.keystore files in root .gitignore

## Phase 3: User Story 2 - Source Control Security (P1)
**Goal**: Generate the signing key and ensure it remains local-only.
**Independent Test**: `git status` must not show the generated keystore file.

- [ ] T003 [US2] Generate singletask.keystore in repository root using keytool
- [ ] T004 [US2] Verify keystore generation and git ignore status via terminal

## Phase 4: User Story 1 - Produce Release Artifact (P1)
**Goal**: Configure build system to produce signed Android App Bundles.
**Independent Test**: `dotnet build -c Release` produces a signed `.aab` in `bin/Release`.

- [ ] T005 [US1] Configure Release signing properties in src/SingleTask/SingleTask.csproj
- [ ] T006 [US1] Run Release build to generate AAB artifact via terminal
- [ ] T007 [US1] Verify generated AAB file existence and signature in bin/Release

## Phase 5: Polish & Cross-Cutting Concerns
**Goal**: Final verification and documentation.

- [ ] T008 Update project documentation with signing instructions in README.md (optional)

## Dependencies

1. **T002 (.gitignore)** must be completed before **T003 (Generate Key)** to prevent accidental commits.
2. **T003 (Generate Key)** must be completed before **T005 (Config .csproj)** as the config references the file.
3. **T005 (Config .csproj)** must be completed before **T006 (Build)**.

## Parallel Execution Examples

- None. This feature is strictly sequential due to security dependencies (Ignore -> Generate -> Configure -> Build).

## Implementation Strategy

1. **Secure First**: We prioritize `.gitignore` updates (T002) to strictly prevent security leaks.
2. **Generate Artifacts**: Create the keystore (T003) once the safety net is in place.
3. **Configure & Build**: Finally, wire up the build system (T005) and verify the output (T006).
