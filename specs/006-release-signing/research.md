# Research: Release Signing (Keystore & AAB)

**Feature**: 006-release-signing
**Date**: 2026-01-07

## Key Decisions

### 1. Keystore Generation Strategy
**Decision**: Use Java `keytool` with RSA 2048-bit encryption.
**Rationale**: Standard for Android signing. 
**Command**:
```bash
keytool -genkeypair -v -keystore singletask.keystore -alias singletask_key -keyalg RSA -keysize 2048 -validity 10000
```
**Details**:
- Validity: 10000 days (approx 27 years) as required.
- Store/Key Password: `[REDACTED]` (from requirements).
- Alias: `singletask_key`

### 2. Build Configuration
**Decision**: Configure `.csproj` with conditional PropertyGroup for Release.
**Rationale**: Ensures signing only happens during release builds, keeping debug builds fast and using default debug keys.
**Configuration**:
```xml
<PropertyGroup Condition="'$(Configuration)' == 'Release'">
  <AndroidKeyStore>True</AndroidKeyStore>
  <AndroidSigningKeyStore>$(ProjectDir)..\singletask.keystore</AndroidSigningKeyStore>
  <AndroidSigningStorePass>[REDACTED]</AndroidSigningStorePass>
  <AndroidSigningKeyAlias>singletask_key</AndroidSigningKeyAlias>
  <AndroidSigningKeyPass>[REDACTED]</AndroidSigningKeyPass>
  <AndroidPackageFormats>aab</AndroidPackageFormats>
</PropertyGroup>
```
*Note*: The keystore path needs to be relative to the project file. If the keystore is in the repo root and the csproj is in `src/SingleTask`, the path is `../../singletask.keystore`. Wait, looking at file structure:
Root: `C:\ProjetAntigravity\SingleTaskRound2`
Project: `C:\ProjetAntigravity\SingleTaskRound2\src\SingleTask\SingleTask.csproj`
So path is `../../singletask.keystore`.

### 3. Security
**Decision**: Add `*.keystore` to `.gitignore`.
**Rationale**: Prevents accidental commit of the private key.

## Alternatives Considered

- **CI/CD Variable Injection**: Using environment variables for passwords.
  - *Rejected*: Current requirement specifies a fixed password for this task. Moving to CI variables is a future enhancement.
- **APK Generation**:
  - *Rejected*: Requirement specifically asks for `.aab`.
