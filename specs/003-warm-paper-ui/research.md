# Phase 0 Research: UI Overhaul - "Warm Paper" Design System

**Date**: 2025-12-14
**Feature**: `003-warm-paper-ui`

## Codebase Analysis

### 1. Global Resources (`Colors.xaml`, `Styles.xaml`)
- **Location**: `src/SingleTask/Resources/Styles/`
- **Findings**:
    - `Colors.xaml` defines the palette. We need to overwrite `PaperBackground`, `CardSurface`, `InkBlack`, etc., with the new HEX values.
    - `Styles.xaml` applies these.
    - **Crucial**: The new specification requires specific fonts (`Playfair Display`, `Inter`). Currently only `OpenSans` exists. We must add the `.ttf` files to `Resources/Fonts` and register them in `MauiProgram.cs`.

### 2. PlanningPage (`PlanningPage.xaml`)
- **Current State**: Uses standard MAUI controls.
- **Refactor Plan**:
    - Remove `CollectionView` if present (or list mechanism).
    - Implement `ScrollView` + `VerticalStackLayout` + `BindableLayout` as per strict technical constraint.
    - Header needs to be updated to "Good Morning" style.

### 3. FocusPage (`FocusPage.xaml`)
- **Current State**: Exists.
- **Refactor Plan**:
    - Needs huge typography updates.
    - Needs to support the "Done" interaction that triggers navigation.

### 4. Navigation & New Page
- **AppShell**: Currently handles basic navigation.
- **New Page**: `CelebrationPage.xaml` does not exist.
    - Needs to be created in `src/SingleTask/Views/` (or `src/SingleTask/` if following flat structure, but `Views/` is preferred if it exists).
    - Note: `PlanningPage` is in root `src/SingleTask/` but `FocusPage` is in `src/SingleTask/Views/`. We should probably organize this better, but for now follow existing patterns or put new page in `Views/`.
- **Flow**:
    - `PlanningViewModel` navigates to `FocusPage`.
    - `FocusViewModel` navigates to `CelebrationPage`.
    - `CelebrationViewModel` navigates back to `PlanningPage` (clearing stack?).

### 5. Data Model (`TaskItem.cs`)
- **Location**: `src/SingleTask.Core/Models/TaskItem.cs`
- **Findings**: Supports `IsCompleted`. This is sufficient.

## Risk Assessment
- **Font Rendering**: Need to ensure fonts are loaded correctly on Android.
- **ScrollView Performance**: With many tasks, `BindableLayout` inside `ScrollView` might be less performant than `CollectionView`, but stability is the priority constraint.
