# Research: Security Compliance

**Feature**: 007-security-compliance
**Date**: 2026-01-09

## 1. Database Encryption (SQLCipher)

**Problem**: User data is stored in plain text SQLite.
**Decision**: Use `sqlite-net-sqlcipher` instead of `sqlite-net-pcl`.
**Rationale**: 
- `sqlite-net-sqlcipher` is a drop-in replacement for `sqlite-net-pcl` that adds full-database encryption.
- It is widely supported and industry standard for local mobile databases.
- Compatible with .NET MAUI.

**Implementation Details**:
- **Package**: `sqlite-net-sqlcipher` (Replace existing package).
- **Initialization**:
  ```csharp
  var options = new SQLiteConnectionString(dbPath, true, key: "password-from-secure-storage");
  _database = new SQLiteAsyncConnection(options);
  ```
- **Key Storage**: Use `Microsoft.Maui.Storage.SecureStorage` to generate (Guid) and store a persistent key on first run.

**Alternatives Considered**:
- *System.Security.Cryptography* to encrypt specific columns: rejected because it's complex to manage and query (no `LIKE` or sorting on encrypted fields).
- *Realm* with encryption: rejected because it requires a full database migration/rewrite from SQLite.

## 2. Secure Backups

**Problem**: `android:allowBackup="true"` allows ADB extraction of unencrypted data.
**Decision**: Disable backups entirely for this MVP (`android:allowBackup="false"`).
**Rationale**:
- Encrypted databases cannot be easily restored to a different device (key in hardware keystore is not transferable).
- Restoring the DB file without the key renders it useless.
- Simplest secure default.

**Alternatives Considered**:
- *Custom Backup Rules*: Excluding just the DB file. This is valid but adds complexity. Disabling is safer for now given the privacy focus.

## 3. Application Hardening

**Problem**: Release builds are debuggable/readable.
**Decision**: Enable R8 shrinking and obfuscation.
**Rationale**: Standard Android practice for release builds.

**Implementation Details**:
Update `SingleTask.csproj` for Release config:
```xml
<PropertyGroup Condition="$(TargetFramework.Contains('-android')) and '$(Configuration)' == 'Release'">
    <AndroidLinkMode>SdkOnly</AndroidLinkMode> <!-- or Full if safe -->
    <PublishTrimmed>true</PublishTrimmed>
    <RunAOTCompilation>true</RunAOTCompilation>
    <AndroidEnableProfiledAot>true</AndroidEnableProfiledAot>
    <AndroidEnableProguard>true</AndroidEnableProguard> <!-- Triggers R8 -->
</PropertyGroup>
```

## 4. Input Validation & Rate Limiting

**Problem**: No protection against DoS via UI input.
**Decision**: Implement checks in ViewModel.
**Rationale**: Validating at the entry point protects all downstream layers.

**Implementation Details**:
- **Max Length**: 500 chars for Task Title.
- **Rate Limit**: Simple timestamp check (`DateTime.UtcNow - _lastAction > TimeSpan.FromMilliseconds(500)`).

## 5. Service Security

**Problem**: `FocusSessionService` implicitly exported? (Defaults vary by API level, but explicit is best).
**Decision**: Explicitly set `Exported = false`.
**Rationale**: Prevents other apps from starting/binding to our internal service.

## 6. Known Unknowns Resolved

- **Migration**: SQLCipher can encrypt an existing plain database using `sqlcipher_export`.
  - *Strategy*: On init, check if `SingleTask.db3` exists. Try opening with key. If fails, try opening with empty key. If success, run migration (export to new encrypted file, replace old).
  - *Refined Strategy*: For this specific MVP, since we don't have a massive user base, we might skip complex in-place migration code if `sqlite-net-sqlcipher` handles it, or simply accept that users might need to reinstall if we change the format drastically. **However**, the best UX is to attempt migration.
  - *Simplified Migration for Plan*:
    1. Check if `SingleTask.db3` exists.
    2. Try to connect with Key. Success -> Done.
    3. Fail -> It might be plaintext. Rename to `backup.db3`.
    4. Create new Encrypted `SingleTask.db3`.
    5. Read from `backup.db3` (plain), write to `SingleTask.db3` (encrypted).
    6. Delete `backup.db3`.
