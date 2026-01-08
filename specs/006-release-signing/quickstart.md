# Quickstart: Release Signing

**Feature**: 006-release-signing

## Prerequisites

- Java Development Kit (JDK) installed and `keytool` in PATH.
- .NET 8 SDK and MAUI workload installed.

## How to Generate Keystore (One-time)

If the keystore is missing, generate it from the repository root:

```powershell
keytool -genkeypair -v -keystore singletask.keystore -alias singletask_key -keyalg RSA -keysize 2048 -validity 10000 -storepass [REDACTED] -keypass [REDACTED] -dname "CN=SingleTask, OU=Mobile, O=Antigravity, L=Paris, S=IDF, C=FR"
```

## How to Build Release

Run the following command from the repository root:

```powershell
dotnet build src/SingleTask/SingleTask.csproj -c Release -p:AndroidPackageFormat=aab
```

## Verify Artifact

1. Navigate to `src/SingleTask/bin/Release/net8.0-android/`.
2. Look for the `-Signed.aab` file.
3. (Optional) Verify signature:
   ```powershell
   jarsigner -verify -verbose -certs src/SingleTask/bin/Release/net8.0-android/*-Signed.aab
   ```
