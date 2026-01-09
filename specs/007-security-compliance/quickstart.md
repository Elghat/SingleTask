# Quickstart: Security Compliance Feature

**Branch**: `007-security-compliance`

## Prerequisites

1.  **Environment Variables**:
    To build the Release version locally, you must set these PowerShell variables (or use the provided keystore if testing generic signing):
    ```powershell
    $env:ANDROID_KEYSTORE_PATH = "C:\path\to\your.keystore"
    $env:ANDROID_KEYSTORE_PASSWORD = "your-password"
    $env:ANDROID_KEY_ALIAS = "your-alias"
    $env:ANDROID_KEY_PASSWORD = "your-password"
    ```

## Running the App

### Debug Mode (Standard)
1.  Open `SingleTask.slnx` in Visual Studio or VS Code.
2.  Run `dotnet build -t:Run -f net10.0-android`.
3.  **Note**: In Debug mode, the database will be encrypted, but obfuscation is disabled.

### Release Mode (Security Verification)
To verify R8 obfuscation and log removal:
1.  Run the build command:
    ```bash
    dotnet build -c Release -f net10.0-android
    ```
2.  Deploy to device/emulator manually or via `dotnet build -t:Run -c Release ...`.
3.  Check Logcat: `adb logcat | grep "SingleTask"` to ensure no debug logs appear.

## Verifying Encryption

1.  Run the app and create a task.
2.  Find the DB path (printed in Debug logs usually, or check `AppDataDirectory`).
3.  Try to pull it:
    ```bash
    adb exec-out run-as com.kingjo.singletask cat files/SingleTask.db3 > local_db.db3
    ```
4.  Open `local_db.db3` in "DB Browser for SQLite".
    - **Expected**: It asks for a password. Providing an incorrect password fails.

## Verifying Backups

1.  Run:
    ```bash
    adb backup -f backup.ab com.kingjo.singletask
    ```
2.  **Expected**: The command might create a 0-byte file or a file containing only manifest data, NOT the database (depending on OS version and `allowBackup` flag).
