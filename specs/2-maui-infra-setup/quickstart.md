# Quickstart: Project Setup

**Feature**: `2-maui-infra-setup`

## Prerequisites
- .NET 10 SDK installed
- Visual Studio 2022+ / VS Code with C# Dev Kit
- Android Emulator configured (API 34+)

## Build & Run

### Android
```bash
cd src/SingleTask
dotnet build -f net10.0-android
dotnet run -f net10.0-android
```

### Windows
```bash
cd src/SingleTask
dotnet build -f net10.0-windows10.0.19041.0
dotnet run -f net10.0-windows10.0.19041.0
```

## Running Tests
```bash
dotnet test tests/SingleTask.UnitTests
```

## Verification
1. App launches with "Home" page.
2. Console logs indicate "Database Initialized".
