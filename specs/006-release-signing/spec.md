# Feature Specification: Release Signing (Keystore & AAB)

**Feature Branch**: `006-release-signing`
**Created**: 2026-01-07
**Status**: Draft
**Input**: User description: Release Signing (Keystore & AAB)

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Produce Release Artifact (Priority: P1)

As a Release Engineer or Developer, I want to build a signed Android App Bundle (AAB) in Release configuration so that I can upload the application to the Google Play Store.

**Why this priority**: Essential for publishing the application.

**Independent Test**: Can be tested by running the release build command and verifying the output signature.

**Acceptance Scenarios**:

1. **Given** the source code and a clean environment, **When** I run `dotnet build -c Release -p:AndroidPackageFormat=aab`, **Then** the build succeeds and a `.aab` file is created in `bin/Release`.
2. **Given** the produced `.aab` file, **When** I verify its signature, **Then** it is signed with the expected `singletask_key`.
3. **Given** the generated keystore, **When** I check its validity, **Then** it is valid for 10000 days.

---

### User Story 2 - Source Control Security (Priority: P1)

As a Developer, I want to ensure the signing keys are not committed to version control so that the security of the application is maintained.

**Why this priority**: Critical security requirement to prevent credential leakage.

**Independent Test**: Can be tested by running `git status` after generating the key.

**Acceptance Scenarios**:

1. **Given** a generated `singletask.keystore` in the root directory, **When** I run `git status`, **Then** the keystore file is ignored (not listed as untracked).

### Edge Cases

- **Build Failure**: If the keystore is missing, the Release build should fail or prompt, but must not produce an unsigned release artifact silently.
- **Debug Build**: Debug builds should continue to use the default debug keystore.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The system MUST contain a valid Java Keystore (`singletask.keystore`) in the project root.
- **FR-002**: The keystore MUST have an alias `singletask_key` and a validity of 10000 days.
- **FR-003**: The build system MUST automatically sign the artifact when building in `Release` configuration.
- **FR-004**: The build output for Release configuration MUST be an Android App Bundle (`.aab`).
- **FR-005**: The build system MUST NOT sign `Debug` configuration builds with the release key.
- **FR-006**: The `singletask.keystore` file MUST be excluded from version control.

### Key Entities

- **Keystore**: A binary file containing the private key for signing.
- **Build Configuration**: The project configuration (.csproj) defining build properties.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: A valid `.aab` file exists in the release output directory after a release build.
- **SC-002**: The `singletask.keystore` file is present in the root but ignored by git.
- **SC-003**: The keystore certificate validity period is 10000 days.
