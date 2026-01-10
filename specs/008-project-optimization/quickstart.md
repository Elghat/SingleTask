# Quickstart: Project Optimization Overhaul

**Feature**: 008-project-optimization  
**Date**: 2026-01-10  
**Estimated Effort**: ~13 hours

## Prerequisites

Before starting implementation:

1. **Capture baseline metrics**:
   ```powershell
   # Build release APK and note size
   dotnet publish -f net10.0-android -c Release
   
   # Record: ____ MB (APK size)
   # Record: ____ ms (cold startup on test device)
   # Record: ____ ms (reorder 20 tasks)
   ```

2. **Ensure tests pass**:
   ```powershell
   dotnet test tests/SingleTask.UnitTests
   ```

3. **Verify on branch**:
   ```powershell
   git branch  # Should show: * 008-project-optimization
   ```

---

## Implementation Order

Follow this sequence for lowest risk and incremental validation:

### Phase 1: Critical Stability Fixes (P1)

#### Step 1.1: Fix async void in FocusViewModel (FR-001)

**File**: `src/SingleTask.Core/ViewModels/FocusViewModel.cs`

```csharp
// Change line 78 from:
public async void Initialize(TaskItem task)

// To:
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
#if DEBUG
        System.Diagnostics.Debug.WriteLine($"InitializeAsync failed: {ex}");
#endif
    }
}
```

**Update constructor** (line ~52):
```csharp
if (_focusService.CurrentFocusedTask != null)
{
    _ = InitializeAsync(_focusService.CurrentFocusedTask);
}
```

**Validate**: Run unit tests for FocusViewModel.

#### Step 1.2: Fix Thread.Sleep in AudioService (FR-002)

**File**: `src/SingleTask/Platforms/Android/Services/AudioService.cs`

```csharp
// Change PlayCelebrationSoundAsync to:
public async Task PlayCelebrationSoundAsync()
{
    try
    {
        var toneGen = new ToneGenerator(global::Android.Media.Stream.Music, 100);
        toneGen.StartTone(Tone.Dtmf1, 200);
        await Task.Delay(250);
        toneGen.StartTone(Tone.Dtmf3, 200);
        await Task.Delay(250);
        toneGen.StartTone(Tone.Dtmf5, 600);
        
        // FR-015: Dispose ToneGenerator
        toneGen.Release();
    }
    catch (Exception ex)
    {
#if DEBUG
        System.Diagnostics.Debug.WriteLine($"Audio Error: {ex.Message}");
#endif
    }
}
```

**Validate**: Run app, complete all tasks, verify UI stays responsive.

---

### Phase 2: Performance Fixes (P1)

#### Step 2.1: Add Batch Save to DatabaseService (FR-004)

**File**: `src/SingleTask.Core/Services/IDatabaseService.cs`

Add method signature:
```csharp
Task SaveTasksAsync(IEnumerable<TaskItem> tasks);
```

**File**: `src/SingleTask.Core/Services/DatabaseService.cs`

Add implementation:
```csharp
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
```

**File**: `src/SingleTask.Core/ViewModels/PlanningViewModel.cs`

Update `SaveTaskOrderAsync`:
```csharp
private async Task SaveTaskOrderAsync()
{
    for (int i = 0; i < Tasks.Count; i++)
        Tasks[i].Order = i;
    
    await _databaseService.SaveTasksAsync(Tasks);
}
```

**Validate**: Create 20 tasks, reorder, verify speed improvement.

#### Step 2.2: Replace BindableLayout with CollectionView (FR-005)

**File**: `src/SingleTask/Views/PlanningPage.xaml`

Replace lines 32-164 (ScrollView + BindableLayout):
```xml
<!--  Row 1: Task List (CollectionView with virtualization)  -->
<CollectionView Grid.Row="1" 
                Margin="0,20"
                ItemsSource="{Binding Tasks}"
                SelectionMode="None">
    <CollectionView.ItemsLayout>
        <LinearItemsLayout Orientation="Vertical" ItemSpacing="12" />
    </CollectionView.ItemsLayout>
    <CollectionView.ItemTemplate>
        <DataTemplate x:DataType="models:TaskItem">
            <!-- Keep existing Border/Grid template unchanged -->
            <Border Style="{StaticResource CardSurfaceStyle}">
                <!-- ... existing content ... -->
            </Border>
        </DataTemplate>
    </CollectionView.ItemTemplate>
</CollectionView>
```

**Validate**: Scroll with 50 tasks, verify smooth 60fps.

---

### Phase 3: Code Quality (P2)

#### Step 3.1: Add Exception Logging (FR-003)

**Files to update**:
- `src/SingleTask.Core/Services/DatabaseService.cs` (lines 78, 101, 130)
- `src/SingleTask/Services/HapticService.cs` (lines 13, 26)
- `src/SingleTask/Views/CelebrationPage.xaml.cs` (line 27)

Pattern to apply:
```csharp
catch (Exception ex)
{
#if DEBUG
    System.Diagnostics.Debug.WriteLine($"[ClassName.MethodName] {ex.Message}");
#endif
}
```

#### Step 3.2: Add IAsyncDisposable (FR-014)

**File**: `src/SingleTask.Core/Services/DatabaseService.cs`

```csharp
public class DatabaseService : IDatabaseService, IAsyncDisposable
{
    // ... existing code ...
    
    public async ValueTask DisposeAsync()
    {
        await CloseAsync();
    }
}
```

---

### Phase 4: Build Configuration (P2)

**File**: `src/SingleTask/SingleTask.csproj`

Add to existing Release PropertyGroup (after line 70):
```xml
<!-- Additional Performance Optimizations -->
<RunAOTCompilation>true</RunAOTCompilation>
<TrimMode>full</TrimMode>
<AndroidEnableMarshalMethods>true</AndroidEnableMarshalMethods>
<AndroidEnablePreloadAssemblies>true</AndroidEnablePreloadAssemblies>
```

**Validate**: Full Release build succeeds without errors.

---

### Phase 5: Resource Cleanup (P2)

#### Step 5.1: Remove Unused Files (FR-010, FR-011)

```powershell
# From project root
Remove-Item "src/SingleTask/Views/MainPage.xaml" -ErrorAction SilentlyContinue
Remove-Item "src/SingleTask/Views/MainPage.xaml.cs" -ErrorAction SilentlyContinue
Remove-Item "src/SingleTask.Core/ViewModels/MainViewModel.cs" -ErrorAction SilentlyContinue
Remove-Item "src/SingleTask/Resources/Fonts/OpenSans-Regular.ttf" -ErrorAction SilentlyContinue
Remove-Item "src/SingleTask/Resources/Fonts/OpenSans-Semibold.ttf" -ErrorAction SilentlyContinue
```

**Update MauiProgram.cs** - Remove registrations:
```csharp
// DELETE these lines:
// builder.Services.AddSingleton<SingleTask.Core.ViewModels.MainViewModel>();
// builder.Services.AddSingleton<SingleTask.Views.MainPage>();
```

#### Step 5.2: Update .gitignore (FR-012)

Add to `.gitignore`:
```
# Build artifacts
*.log
preprocessed.xml
```

Remove existing files:
```powershell
Remove-Item "src/SingleTask/build_debug.log" -ErrorAction SilentlyContinue
Remove-Item "src/SingleTask/build_error_3.log" -ErrorAction SilentlyContinue
Remove-Item "src/SingleTask/preprocessed.xml" -ErrorAction SilentlyContinue
```

---

### Phase 6: Consolidation (P3)

#### Step 6.1: Remove Duplicate Service (FR-016)

**File to delete**: `src/SingleTask/Platforms/Android/Services/FocusForegroundService.cs`

Verify `FocusSessionService.cs` is the one being used in `AndroidFocusService.cs`.

#### Step 6.2: Standardize ObservableProperty (FR-018)

If `MainViewModel.cs` is not deleted (optional), convert:
```csharp
// From manual property:
private string countText = "Click me";
public string CountText
{
    get => countText;
    set => SetProperty(ref countText, value);
}

// To source-generated:
[ObservableProperty]
private string countText = "Click me";
```

---

## Validation Checklist

After all phases:

- [ ] All unit tests pass
- [ ] App launches without crash
- [ ] Task reordering feels instant
- [ ] Scrolling is smooth with 50+ tasks
- [ ] Celebration sound plays without freeze
- [ ] Release build succeeds
- [ ] APK size reduced by 30%+
- [ ] No compiler warnings

---

## Rollback Plan

If issues arise:
```powershell
git checkout main -- src/
git checkout main -- .gitignore
```

Individual file rollback:
```powershell
git checkout main -- path/to/file
```
