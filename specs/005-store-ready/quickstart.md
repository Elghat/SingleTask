# Quickstart: Building for Store

**Feature**: `005-store-ready`

## Prerequisites
*   Android SDK installed (API 31+ for Android 12 features).
*   Keystore file (if signing locally - though not covered by this agent's scope, the config must support it).

## Build Command
To build the release version (unsigned or signed with debug key for testing):

```bash
dotnet build src/SingleTask/SingleTask.csproj -c Release -f net8.0-android
```

## Verification Steps
1.  **Check Manifest**:
    *   Open `src/SingleTask/obj/Release/net8.0-android/AndroidManifest.xml`
    *   Verify `package="com.gragware.singletask"`
    *   Verify `android:versionName="1.0.0"`

2.  **Check Splash Screen**:
    *   Deploy to Emulator (Android 12+):
        ```bash
        dotnet build src/SingleTask/SingleTask.csproj -t:Run -c Release -f net8.0-android
        ```
    *   Observe startup animation and background color (`#fdfbf7`).
