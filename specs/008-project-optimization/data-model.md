# Data Model: Project Optimization Overhaul

**Feature**: 008-project-optimization  
**Date**: 2026-01-10  
**Phase**: 1 - Design

## Overview

This feature primarily involves code quality and performance optimizations rather than data model changes. The only data model impact is an interface extension for batch database operations.

---

## Entity Changes

### TaskItem (No Changes)

The existing `TaskItem` entity remains unchanged. Reference for context:

```csharp
// SingleTask.Core/Models/TaskItem.cs - NO CHANGES
public partial class TaskItem : ObservableObject
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    public string Title { get; set; } = string.Empty;
    public TaskStatus Status { get; set; } = TaskStatus.Pending;
    public int Order { get; set; }
    
    // Computed/UI properties (not persisted)
    [Ignore] public bool IsCompleted => Status == TaskStatus.Completed;
    [Ignore] public int DisplayIndex { get; set; }
    [Ignore] public bool IsTop { get; set; }
    [Ignore] public bool CanMoveUp => !IsCompleted && !IsTop;
    [Ignore] public bool CanMoveDown => !IsCompleted && DisplayIndex > 1;
}
```

### TestEntity (To Be Removed/Moved)

```csharp
// SingleTask.Core/Models/TestEntity.cs - REMOVE FROM CORE
// Move to SingleTask.UnitTests if needed for testing
public class TestEntity
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
```

**Action**: Remove from `SingleTask.Core`. If test project needs it, recreate there.

---

## Interface Changes

### IDatabaseService (Extended)

```csharp
// SingleTask.Core/Services/IDatabaseService.cs

public interface IDatabaseService
{
    // Existing methods (unchanged)
    Task<List<TestEntity>> GetItemsAsync();
    Task<int> SaveItemAsync(TestEntity item);
    Task<int> DeleteItemAsync(TestEntity item);
    Task<List<TaskItem>> GetTasksAsync();
    Task<int> SaveTaskAsync(TaskItem item);
    Task<int> DeleteTaskAsync(TaskItem item);
    Task CloseAsync();
    
    // NEW: Batch save for performance (FR-004)
    Task SaveTasksAsync(IEnumerable<TaskItem> tasks);
}
```

### IDatabaseService (Full Contract After Changes)

```csharp
namespace SingleTask.Core.Services;

/// <summary>
/// Database operations for task persistence.
/// SEC-002: Uses SQLCipher encryption.
/// </summary>
public interface IDatabaseService : IAsyncDisposable  // FR-014: Add IAsyncDisposable
{
    /// <summary>Get all tasks ordered by Order field.</summary>
    Task<List<TaskItem>> GetTasksAsync();
    
    /// <summary>Save a single task (insert or update).</summary>
    Task<int> SaveTaskAsync(TaskItem item);
    
    /// <summary>Delete a task.</summary>
    Task<int> DeleteTaskAsync(TaskItem item);
    
    /// <summary>
    /// Batch save multiple tasks in a single transaction.
    /// FR-004: Performance optimization for reordering.
    /// </summary>
    Task SaveTasksAsync(IEnumerable<TaskItem> tasks);
    
    /// <summary>Close database connection.</summary>
    Task CloseAsync();
    
    // Legacy test entity methods (consider removing)
    Task<List<TestEntity>> GetItemsAsync();
    Task<int> SaveItemAsync(TestEntity item);
    Task<int> DeleteItemAsync(TestEntity item);
}
```

---

## Service Changes

### DatabaseService (Implementation)

```csharp
// SingleTask.Core/Services/DatabaseService.cs

public class DatabaseService : IDatabaseService, IAsyncDisposable
{
    private SQLiteAsyncConnection? _database;
    private readonly string _dbPath;
    private readonly ISecureStorageService _secureStorage;
    
    // ... existing code ...
    
    // NEW: FR-004 - Batch save implementation
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
    
    // NEW: FR-014 - IAsyncDisposable implementation
    public async ValueTask DisposeAsync()
    {
        await CloseAsync();
    }
}
```

---

## ViewModel Method Signature Changes

### FocusViewModel

```csharp
// BEFORE
public async void Initialize(TaskItem task)

// AFTER (FR-001)
public async Task InitializeAsync(TaskItem task)
```

**Impact**: Callers must update:
- Constructor: `_ = InitializeAsync(task);` or await properly
- Any direct calls to `Initialize()` method

### PlanningViewModel

No public method signature changes. Internal `SaveTaskOrderAsync` updated to use batch save.

---

## Files to Remove

| File | Reason | Action |
|------|--------|--------|
| `src/SingleTask/Views/MainPage.xaml` | Unused (legacy demo) | Delete |
| `src/SingleTask/Views/MainPage.xaml.cs` | Unused (legacy demo) | Delete |
| `src/SingleTask.Core/ViewModels/MainViewModel.cs` | Unused (legacy demo) | Delete |
| `src/SingleTask.Core/Models/TestEntity.cs` | Only for testing | Move to test project or delete |
| `src/SingleTask/Resources/Fonts/OpenSans-Regular.ttf` | Unused font | Delete |
| `src/SingleTask/Resources/Fonts/OpenSans-Semibold.ttf` | Unused font | Delete |
| `src/SingleTask/Platforms/Android/Services/FocusForegroundService.cs` | Duplicate of FocusSessionService | Delete |

---

## DI Registration Changes

### MauiProgram.cs

```csharp
// REMOVE these registrations after deleting unused files:
// builder.Services.AddSingleton<SingleTask.Core.ViewModels.MainViewModel>();
// builder.Services.AddSingleton<SingleTask.Views.MainPage>();

// No changes needed for DatabaseService - IAsyncDisposable is 
// automatically handled for singletons at app shutdown
```

---

## Validation Rules

No new validation rules. Existing task validation preserved:
- Title max length: 200 characters (SEC-005)
- Rate limiting: 500ms between commands (SEC-012)
- Duplicate detection with user confirmation

---

## State Transitions

No state machine changes. TaskStatus enum unchanged:
```
Pending → Completed (via CompleteTask command)
```

---

## Migration Notes

1. **No database migration needed** - Schema unchanged
2. **No data migration needed** - Only code changes
3. **No user action required** - Seamless update
