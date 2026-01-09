# Implementation Plan: security-compliance

**Branch**: `007-security-compliance` | **Date**: 2026-01-09 | **Spec**: [specs/007-security-compliance/spec.md](../spec.md)
**Input**: Feature specification from `specs/007-security-compliance/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/commands/plan.md` for the execution workflow.

## Summary

This feature implements security hardening for the SingleTask .NET MAUI application based on the 2026 Security Audit Report. The primary goals are to encrypt local data at rest using SQLCipher, disable insecure data backups, enforce input validation to prevent DoS, and apply release build obfuscation. This ensures user privacy and protects against data exfiltration attacks.

## Technical Context

**Language/Version**: C# 12 / .NET 10 (MAUI)
**Primary Dependencies**: 
- `sqlite-net-sqlcipher` (for DB encryption)
- `Microsoft.Maui.Storage.SecureStorage` (for key management)
- `ProGuard` / `R8` (for obfuscation)
**Storage**: SQLite (Encrypted with SQLCipher)
**Testing**: xUnit (Unit Tests), Manual Verification (Security Scenarios)
**Target Platform**: Android (API 26+)
**Project Type**: Mobile Application
**Performance Goals**: < 200ms overhead for DB operations, minimal startup impact
**Constraints**: Must support existing data migration
**Scale/Scope**: Local database encryption, configuration changes, service hardening

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- **Principle 1 (Library-First)**: N/A - Application hardening.
- **Principle 2 (CLI Interface)**: N/A - Mobile app.
- **Principle 3 (Test-First)**: Plan includes unit tests for validators and integration tests for DB.
- **Principle 4 (Integration Testing)**: Essential for encrypted DB verification.
- **Principle 5 (Observability)**: Security observability improved by stripping debug logs.
- **Status**: PASSED.

## Project Structure

### Documentation (this feature)

```text
specs/007-security-compliance/
├── plan.md              # This file
├── research.md          # Implementation details for SQLCipher & R8
├── data-model.md        # Schema changes & Encryption Service definition
├── quickstart.md        # Security verification guide
├── contracts/           # (Empty - no external API changes)
└── tasks.md             # To be created
```

### Source Code (repository root)

```text
src/
├── SingleTask/
│   ├── Platforms/Android/
│   │   ├── AndroidManifest.xml       # Update: allowBackup=false
│   │   └── Services/                 # Update: FocusSessionService (Exported=false)
│   └── SingleTask.csproj             # Update: Release config (R8, Keystore, MinSDK)
├── SingleTask.Core/
│   ├── Services/
│   │   ├── DatabaseService.cs        # Refactor: Inject IEncryptionService, use SQLCipher
│   │   ├── EncryptionService.cs      # New: Key management via SecureStorage
│   │   └── IEncryptionService.cs     # New: Interface
│   └── ViewModels/
│       └── PlanningViewModel.cs      # Update: Input validation & Rate limiting
└── tests/
    └── SingleTask.UnitTests/
        └── Services/
            └── EncryptionServiceTests.cs # New: Tests for key generation
```

**Structure Decision**: Modifying existing architecture to support encryption. Adding `EncryptionService` to handle key lifecycle strictly.

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| N/A       | N/A        | N/A                                 |
