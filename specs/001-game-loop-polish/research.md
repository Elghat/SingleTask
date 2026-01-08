# Research: Game Loop Polish & Interactions

**Feature**: Game Loop Polish & Interactions
**Status**: Completed

## 1. Technical Clarifications

### .NET Version
- **Finding**: Project uses `.NET 10.0.100` SDK (via `global.json`).
- **Implication**: Use standard MAUI patterns compatible with .NET 8/10.

### Missing Dependency: Material Symbols
- **Finding**: The spec states "Icons: Use FontImageSource with Material Symbols (already set up in fonts)".
- **Reality**: `MauiProgram.cs` only registers OpenSans (aliased as Inter/Playfair). `Resources/Fonts` contains only OpenSans.
- **Decision**: **Must add Material Symbols font file**.
- **Action**: 
  1. Download `MaterialSymbolsRounded-Regular.ttf` (or similar).
  2. Register it in `MauiProgram.cs` with alias `MaterialSymbols`.
  3. Define a static class `IconFont.cs` (or similar) or use raw unicode strings in XAML.
  4. Use `FontImageSource FontFamily="MaterialSymbols"`.

## 2. Implementation Approach

### Defer Task Logic
- **Current State**: `TaskItem` has an `Order` property. `GetTasksAsync` orders by this property.
- **Problem**: `Order` might be arbitrary (0 for all?).
- **Logic for Defer**:
  1. Find the MAX `Order` in the current list.
  2. Set the deferred task's `Order` to `MAX + 1`.
  3. Update database.
  4. Navigate back.
- **Alternative**: If `Order` is not reliable, we might need to re-index the whole list.
  - **Selected Approach**: "Max + 1" strategy is sufficient for "Defer to End". If collisions occur, it's acceptable for this feature's scope (simple todo list).

### UI Polish
- **Completed Tasks**:
  - Use `DataTrigger` on `Grid` or `Frame` wrapping the task.
  - Bind to `IsCompleted`.
  - Set `Opacity` to `0.5` and `InputTransparent` to `True`.
- **Icons**:
  - Add `MaterialSymbols` font.
  - Replace text buttons with `ImageButton` or `Button` with `ImageSource`.

### Confirmation Dialog
- **Mechanism**: Use `Shell.Current.DisplayAlert`.
- **Trigger**: Intercept `BackButtonPressed` or override `OnBackButtonPressed` in `FocusPage`. 
- **Note**: `FocusPage` might be modal. If so, need to handle the "Close" button (X) explicitly. Spec mentions "Close (X) button", implying a UI element, not just hardware back.

## 3. Risks & Unknowns
- **Risk**: `Order` property usage might be inconsistent in existing data.
- **Mitigation**: When loading tasks, if `Order` is all 0, re-index them in memory or DB?
  - **Decision**: For now, assume "Max + 1" works. If `Order` is 0 for all, deferred task becomes 1 (end). Works.

