# Research: Google Play Store Preparation

**Feature**: `005-store-ready`
**Status**: Complete

## Findings

### 1. Splash Screen Configuration (Android 12+)
*   **Decision**: Use standard `<MauiSplashScreen>` item in `SingleTask.csproj`.
*   **Rationale**: MAUI handles the generation of the Android 12 Splash Screen API requirements automatically when the `MauiSplashScreen` item is configured.
*   **Configuration**:
    ```xml
    <ItemGroup>
      <MauiSplashScreen Include="Resources\Splash\splash.svg" Color="#fdfbf7" BaseSize="128,128" />
    </ItemGroup>
    ```
    *   `Color` attribute sets the background.
    *   `BaseSize` ensures correct scaling.

### 2. Versioning & Identity
*   **Decision**: Configure properties directly in `SingleTask.csproj`.
*   **Properties**:
    *   `ApplicationId`: `com.gragware.singletask`
    *   `ApplicationDisplayVersion`: `1.0.0`
    *   `ApplicationVersion`: `1`
    *   `ApplicationTitle`: `SingleTask`

### 3. Code Hygiene (Smart Scan Results)
*   **Debug Logging**: Found `System.Diagnostics.Debug.WriteLine` in:
    *   `src\SingleTask\Platforms\Android\Services\AudioService.cs`
    *   `src\SingleTask\Platforms\Android\Services\AndroidFocusService.cs`
*   **Action**: Wrap these in `#if DEBUG` directives or remove them if unnecessary for production.
*   **TODO/FIXME**: No relevant markers found in source code (only in `obj/` folder which is ignored).

## Integration Strategy
*   Modify `SingleTask.csproj` for global metadata and splash screen.
*   Verify `MainActivity.cs` and `MainApplication.cs` attributes do not conflict with `csproj` properties.
*   Perform code cleanup on identified files.
