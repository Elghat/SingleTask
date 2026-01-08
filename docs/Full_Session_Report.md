# SingleTask Project: Implementation & Debugging Report
**Date:** December 13, 2025

## 1. Project Goal: "Paper & Ink" UI
The objective was to transform the generic File->New->MAUI app into a "soulful" SingleTask experience.
-   **Design Philosophy:** Avoid "tech" blues. Use warm, textured "paper" colors (Cream/Off-white) and "ink" (Dark Gray/Black) typography.
-   **Components:**
    -   **Cards:** Rounded corners (28px), subtle shadows, floating effect.
    -   **Typography:** Bold, clean headers (32px), readable body text.
    -   **Interaction:** Micro-interactions (checkmarks, strikethrough).

## 2. Implementation Phase
We successfully established the infrastructure:
-   **MVVM Setup:** Connected `PlanningViewModel` to `PlanningPage`.
-   **DispatcherService:** Implemented thread-safe UI updates (crucial for async operations).
-   **Resources:** Defined `Colors.xaml` and `Styles.xaml` with the new palette.

## 3. The "Add Task" Crash (Android)
Upon implementing the full UI, a critical bug emerged:
-   **Symptom:** The app crashed immediately (`Java.Lang.RuntimeException`) when adding a task to the list on Android.
-   **Context:** This occurred only when the `CollectionView` contained complex "Paper & Ink" items. Simple text items worked fine.

## 4. Debugging & Iterative Testing
We performed a "Subtract to Isolate" strategy (Test A through F) to pinpoint the root cause.

| Test ID | Modification | Result | Conclusion |
| :--- | :--- | :--- | :--- |
| **Test A** | **Safe Mode** (Labels only) | **✅ Stable** | Core logic is fine. Issue is purely UI/Rendering. |
| **Test B** | Border (No Shadow) | ❌ Crash | Shadows are not the cause. |
| **Test C** | Border (No Triggers) | ❌ Crash | The `Border` control itself causes issues in `CollectionView`. |
| **Test D** | Replace `Border` w/ `Frame` | ⚠️ Hang | `Frame` caused a layout deadlock (Infinite Loading). |
| **Test E** | Revert to Safe Mode | **✅ Stable** | Confirmed we can always fallback to stability. |
| **Test F** | Primitives (`Grid` + `Shapes`) | ❌ Crash | Even basic shapes inside nested Grids trigger the renderer crash. |

## 5. Technical Conclusion & Next Steps
**Root Cause:** The `CollectionView` (wrapping Android `RecyclerView`) in the current .NET MAUI version on this device has instability when rendering non-trivial Visual Trees (Borders, nested Grids, Shapes) for its items.

**Recommendation (Test G):**
We should abandon the `CollectionView` for the main task list. Instead, we should use a **`BindableLayout` inside a `VerticalStackLayout`**.
-   **Reasoning:** This avoids the complex "Recycling" logic of Android. It renders items as a simple stack of views.
-   **Trade-off:** Slightly lower performance for lists > 500 items (irrelevant for a daily planner).
-   **Benefit:** 100% Stability ensuring we can implement the full "Paper & Ink" design without crashes.

**Current Status:** App is running in Safe Mode (Text Mode) awaiting the Test G implementation.
