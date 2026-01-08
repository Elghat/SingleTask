# Research: MAUI Infrastructure Setup

**Feature**: `2-maui-infra-setup`
**Date**: 2025-12-11

## Decisions

### 1. Project Initialization
- **Decision**: Use `dotnet new maui -n SingleTask` inside `src/`.
- **Rationale**: Standard Microsoft template ensures correct build targets and configurations for .NET 10.
- **Alternatives**: Manual csproj creation (prone to errors).

### 2. MVVM Framework
- **Decision**: `CommunityToolkit.Mvvm`.
- **Rationale**: Official Microsoft toolkit, high performance (Source Generators), standard for modern .NET.
- **Verification**: Package is `CommunityToolkit.Mvvm`.

### 3. Database
- **Decision**: `sqlite-net-pcl`.
- **Rationale**: Lightweight ORM, standard for Xamarin/MAUI.
- **Dependencies**: Requires `SQLitePCLRaw.bundle_green` for native libraries on Android/Windows.

### 4. Dependency Injection
- **Decision**: `Microsoft.Extensions.DependencyInjection` (Built-in to MAUI).
- **Rationale**: Default in `MauiProgram.cs`, no need for 3rd party container.

### 5. Testing
- **Decision**: xUnit.
- **Rationale**: The de-facto standard for .NET Core/.NET 5+ testing.

## Unknowns Resolved
- **Platform Paths**: Will use `FileSystem.AppDataDirectory` (Essentials) for DB path.

## Open Questions
- None.
