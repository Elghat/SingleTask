# Data Model & Contracts: Game Loop Polish

## Data Model Changes

### TaskItem
No schema changes required. Existing `Order` field will be utilized actively.

```csharp
public class TaskItem
{
    // ... existing fields ...
    public int Order { get; set; } // Usage: Lower = Higher Priority. Defer = MAX(Order) + 1.
}
```

## Logic / Service Contracts

### IDatabaseService Extensions

No signature changes strictly required if we use `SaveTaskAsync`, but a helper might be useful in ViewModel.

**Logic Flow (ViewModel Level):**

```csharp
// In FocusViewModel.cs

public async Task DeferTaskAsync(TaskItem task)
{
    // 1. Get all pending tasks
    var tasks = await _databaseService.GetTasksAsync();
    
    // 2. Calculate new order
    var maxOrder = tasks.Any() ? tasks.Max(t => t.Order) : 0;
    task.Order = maxOrder + 1;
    
    // 3. Save
    await _databaseService.SaveTaskAsync(task);
    
    // 4. Navigate back
    await _navigationService.GoBackAsync();
}
```

## UI Contracts (Resources)

### Font Icons (New)
**Alias**: `MaterialSymbols`
**File**: `MaterialSymbolsRounded.ttf` (to be added)

**Glyphs (Examples):**
- `ArrowUp` (Add): `\ue5d8`
- `Rocket` (Start): `\ueb43`
- `LowPriority` (Defer): `\ue16c`
- `Close` (X): `\ue5cd`

## State Transitions

### Task Lifecycle
1. **Created**: Order = Default (or appended). Status = Pending.
2. **Deferred**: Order updated to `Max + 1`. Status remains Pending.
3. **Completed**: Status = Completed. Visuals = Dimmed/Struck-through.

