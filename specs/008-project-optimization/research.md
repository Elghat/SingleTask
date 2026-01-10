# Research: Project Optimization Overhaul

**Feature**: 008-project-optimization  
**Date**: 2026-01-10  
**Status**: Complete

## Overview

This research document captures technology decisions and best practices for the project optimization feature. Since all technologies are already in use in the project, this focuses on patterns and approaches rather than technology selection.

---

## Research Item 1: Async Void to Async Task Conversion

### Context
- **Source**: FR-001 - `async void` methods in FocusViewModel, PlanningPage, CelebrationPage
- **Problem**: Unhandled exceptions in `async void` crash the application

### Decision
**Convert `async void` to `async Task` for ViewModel methods; use fire-and-forget wrapper for lifecycle methods.**

### Rationale
- `async void` is only appropriate for event handlers (like button clicks)
- ViewModel methods like `Initialize()` should return `Task` for proper exception propagation
- Page lifecycle methods (`OnAppearing`) can use fire-and-forget with explicit error handling

### Implementation Pattern

```csharp
// FocusViewModel.cs - BEFORE
public async void Initialize(TaskItem task) { ... }

// FocusViewModel.cs - AFTER
public async Task InitializeAsync(TaskItem task)
{
    try
    {
        CurrentTask = task;
        TaskTitle = task.Title;
        IsCelebrating = false;
        await UpdateTaskProgressAsync();
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"Initialize failed: {ex}");
        // Optionally notify user via IAlertService
    }
}
```

```csharp
// Page lifecycle - fire-and-forget with error handling
protected override void OnAppearing()
{
    base.OnAppearing();
    _ = LoadDataAsync().ContinueWith(t =>
    {
        if (t.IsFaulted)
            System.Diagnostics.Debug.WriteLine($"OnAppearing error: {t.Exception}");
    }, TaskScheduler.FromCurrentSynchronizationContext());
}
```

### Alternatives Considered

| Alternative | Rejected Because |
|------------|------------------|
| Keep async void + global handler | Global handlers can't recover gracefully |
| Use `SafeFireAndForget` extension | Adds external dependency |
| Wrap in MainThread.BeginInvoke | Doesn't solve exception propagation |

---

## Research Item 2: Batch Database Operations

### Context
- **Source**: FR-004 - SaveTaskOrderAsync performs N writes for N tasks
- **Problem**: Sequential database writes cause noticeable UI lag

### Decision
**Use SQLite transaction wrapping with `RunInTransactionAsync`.**

### Rationale
- sqlite-net-sqlcipher (current dependency) supports transactions natively
- Single transaction reduces I/O overhead from N operations to 1
- Atomic operation ensures consistency on failure

### Implementation Pattern

```csharp
// IDatabaseService.cs - Add new method
Task SaveTasksAsync(IEnumerable<TaskItem> tasks);

// DatabaseService.cs - Implementation
public async Task SaveTasksAsync(IEnumerable<TaskItem> tasks)
{
    await InitAsync();
    await _database!.RunInTransactionAsync(db =>
    {
        foreach (var task in tasks)
        {
            if (task.Id != 0)
                db.Update(task);
            else
                db.Insert(task);
        }
    });
}

// PlanningViewModel.cs - Updated caller
private async Task SaveTaskOrderAsync()
{
    for (int i = 0; i < Tasks.Count; i++)
        Tasks[i].Order = i;
    
    await _databaseService.SaveTasksAsync(Tasks); // Single call
}
```

### Performance Expectation
- Current: ~50ms per task × 20 tasks = 1000ms
- After: ~50ms total (single transaction)
- **Expected improvement: 20x faster**

### Alternatives Considered

| Alternative | Rejected Because |
|------------|------------------|
| Background thread saves | Still N operations, adds complexity |
| Debounce saves | Delays persistence, data loss risk |
| In-memory caching | Complicates consistency model |

---

## Research Item 3: CollectionView Virtualization

### Context
- **Source**: FR-005 - BindableLayout renders all items in memory
- **Problem**: Scroll jank with 20+ items

### Decision
**Replace BindableLayout with CollectionView using LinearItemsLayout.**

### Rationale
- CollectionView is the recommended replacement for ListView in .NET MAUI
- Built-in virtualization recycles item views
- Same binding capabilities as BindableLayout

### Implementation Pattern

```xml
<!-- BEFORE: BindableLayout -->
<ScrollView>
    <VerticalStackLayout BindableLayout.ItemsSource="{Binding Tasks}">
        <BindableLayout.ItemTemplate>
            <DataTemplate>...</DataTemplate>
        </BindableLayout.ItemTemplate>
    </VerticalStackLayout>
</ScrollView>

<!-- AFTER: CollectionView -->
<CollectionView ItemsSource="{Binding Tasks}"
                SelectionMode="None">
    <CollectionView.ItemsLayout>
        <LinearItemsLayout Orientation="Vertical" 
                           ItemSpacing="12" />
    </CollectionView.ItemsLayout>
    <CollectionView.ItemTemplate>
        <DataTemplate>...</DataTemplate>
    </CollectionView.ItemTemplate>
</CollectionView>
```

### Considerations
- Remove enclosing `ScrollView` (CollectionView handles scrolling)
- `SelectionMode="None"` since we use explicit buttons for actions
- Test reorder button bindings work correctly within DataTemplate

### Alternatives Considered

| Alternative | Rejected Because |
|------------|------------------|
| Keep BindableLayout + pagination | Adds UX complexity |
| Use CarouselView | Wrong UX pattern for task lists |
| Custom virtualization | Reinvents existing solution |

---

## Research Item 4: Build Configuration Optimization

### Context
- **Source**: FR-006 to FR-009 - Missing AOT, trimming, marshal methods
- **Problem**: Suboptimal cold startup and APK size

### Decision
**Add comprehensive Release build optimizations to .csproj.**

### Rationale
- .NET 10 SDK provides these optimizations with simple MSBuild properties
- No runtime changes required
- Documented startup improvements of 200-500ms

### Implementation Pattern

```xml
<!-- Add to SingleTask.csproj, Release PropertyGroup -->
<PropertyGroup Condition="$(TargetFramework.Contains('-android')) and '$(Configuration)' == 'Release'">
    <!-- Explicit AOT (in addition to existing ProfiledAot) -->
    <RunAOTCompilation>true</RunAOTCompilation>
    
    <!-- Full trimming for .NET 10 -->
    <TrimMode>full</TrimMode>
    
    <!-- Marshal methods optimization -->
    <AndroidEnableMarshalMethods>true</AndroidEnableMarshalMethods>
    
    <!-- Startup tracing -->
    <AndroidEnablePreloadAssemblies>true</AndroidEnablePreloadAssemblies>
</PropertyGroup>
```

### Risk Mitigation
- Run full test suite after enabling TrimMode=full
- Monitor for reflection-related runtime errors
- Add `[DynamicallyAccessedMembers]` attributes if needed

### Alternatives Considered

| Alternative | Rejected Because |
|------------|------------------|
| NativeAOT | Not fully supported for MAUI Android |
| Partial trimming only | Leaves optimization on the table |
| R8 shrinker only | Doesn't provide .NET-level optimizations |

---

## Research Item 5: Thread.Sleep Replacement

### Context
- **Source**: FR-002 - AudioService uses blocking Thread.Sleep
- **Problem**: Blocks thread pool, potential ANR

### Decision
**Replace Thread.Sleep with await Task.Delay.**

### Rationale
- `Task.Delay` is the async-native equivalent
- Does not block thread pool threads
- Same timing behavior without blocking

### Implementation Pattern

```csharp
// BEFORE
toneGen.StartTone(Tone.Dtmf1, 200);
Thread.Sleep(250);
toneGen.StartTone(Tone.Dtmf3, 200);

// AFTER
toneGen.StartTone(Tone.Dtmf1, 200);
await Task.Delay(250);
toneGen.StartTone(Tone.Dtmf3, 200);
```

### Note
Method signature changes from `Task.Run(() => { ... })` to proper async:
```csharp
public async Task PlayCelebrationSoundAsync()
{
    // Direct async implementation
}
```

---

## Research Item 6: IAsyncDisposable Pattern

### Context
- **Source**: FR-014 - DatabaseService holds SQLiteAsyncConnection
- **Problem**: No proper disposal of database connection

### Decision
**Implement IAsyncDisposable on DatabaseService.**

### Rationale
- `IAsyncDisposable` is the standard for async cleanup
- SQLite connections should be properly closed
- Enables `await using` pattern for consumers

### Implementation Pattern

```csharp
public class DatabaseService : IDatabaseService, IAsyncDisposable
{
    private SQLiteAsyncConnection? _database;
    
    public async ValueTask DisposeAsync()
    {
        if (_database != null)
        {
            await _database.CloseAsync();
            _database = null;
        }
    }
}
```

### DI Registration Note
For singletons, disposal happens at app shutdown. Consider explicit `CloseAsync()` call in app lifecycle if needed.

---

## Summary

| Research Item | Decision | Confidence |
|--------------|----------|------------|
| Async Void Conversion | async Task + try-catch | High |
| Batch Database | RunInTransactionAsync | High |
| List Virtualization | CollectionView | High |
| Build Config | AOT + TrimMode=full | Medium-High |
| Thread.Sleep | Task.Delay | High |
| Disposal Pattern | IAsyncDisposable | High |

All research items resolved. No NEEDS CLARIFICATION markers remain.
