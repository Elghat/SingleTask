# Build & Release Procedure

This document describes the commands used to build, test, and release the SingleTask app.

## Prerequisites

- .NET 10 SDK
- Android SDK (API 26+)
- Java JDK (for bundletool)
- [Bundletool](https://github.com/google/bundletool/releases) (optional, for local AAB testing)

## Environment Variables

Set these in your PowerShell profile for signing:

```powershell
$env:ANDROID_KEYSTORE_PATH = "C:\path\to\singletask_release.keystore"
$env:ANDROID_KEYSTORE_PASSWORD = "your-password"
$env:ANDROID_KEY_PASSWORD = "your-password"
```

---

## Development Build (Debug)

```powershell
dotnet build src\SingleTask -c Debug -f net10.0-android
```

---

## Unit Tests

```powershell
dotnet test tests\SingleTask.UnitTests -v n --nologo
```

---

## Release APK (Local Testing)

Generates an APK with full Release optimizations (linking, AOT, trimming):

```powershell
dotnet publish src\SingleTask -c Release -f net10.0-android -p:AndroidPackageFormats=apk
```

Output: `src\SingleTask\bin\Release\net10.0-android\publish\com.kingjo.singletask-Signed.apk`

Install on device/emulator:

```powershell
adb install -r "src\SingleTask\bin\Release\net10.0-android\publish\com.kingjo.singletask-Signed.apk"
```

---

## Release AAB (Google Play Console)

Generates the App Bundle for Google Play submission:

```powershell
dotnet publish src\SingleTask -c Release -f net10.0-android
```

Output: `src\SingleTask\bin\Release\net10.0-android\publish\com.kingjo.singletask-Signed.aab`

---

## Testing AAB Locally with Bundletool

Instead of uploading to Google Play Console every time, use bundletool to test AAB locally:

### 1. Build the AAB

```powershell
dotnet publish src\SingleTask -c Release -f net10.0-android
```

### 2. Convert AAB to APKs

```powershell
java -jar tools\bundletool.jar build-apks `
  --bundle="src\SingleTask\bin\Release\net10.0-android\publish\com.kingjo.singletask-Signed.aab" `
  --output="singletask.apks" `
  --mode=universal `
  --ks="$env:ANDROID_KEYSTORE_PATH" `
  --ks-key-alias="singletask_key" `
  --ks-pass="pass:$env:ANDROID_KEYSTORE_PASSWORD" `
  --key-pass="pass:$env:ANDROID_KEY_PASSWORD"
```

### 3. Install on Device/Emulator

```powershell
java -jar tools\bundletool.jar install-apks --apks=singletask.apks
```

---

## Increment Version

Before each release, update `ApplicationVersion` in `src\SingleTask\SingleTask.csproj`:

```xml
<ApplicationVersion>8</ApplicationVersion>
```

---

## Cleanup

Remove build artifacts:

```powershell
dotnet clean src\SingleTask
Remove-Item -Recurse -Force src\SingleTask\bin, src\SingleTask\obj
```
