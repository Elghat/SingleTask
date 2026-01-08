# 🧪 QA Torture Test Plan: "Breaking SingleTask"

You asked for it! Here are the best ways to try and break the app. Use these to verify robustness.

## 1. The "Spam Clicker" (Concurrency Test)
**Goal:** Break the state machine.
1. Add 3 tasks.
2. Start Focus Mode.
3. **Rapidly tap** the "Complete Task" button (use two fingers if possible) as fast as you can.
**Expected:** It should handle inputs gracefully.
**Broken if:** It crashes, skips a task, or plays the celebration sound while still showing a pending task.

## 2. The "Text Bomb" (UI Overflow Test)
**Goal:** Break the layout.
1. Create a task with a massive title: *paste a whole paragraph of text*.
2. Create a task with no spaces: `WWWWWWWWWWWWWWWWWWWWWWWWWWWWWW` (30+ W's).
**Expected:** Text should wrap or truncate. Button shouldn't be pushed off-screen. Confetti/Checkmarks shouldn't misalign.
**Broken if:** Text goes off-screen, buttons disappear, or the app crashes.

## 3. The "Ghost Button" (Binding Test)
**Goal:** Verify the "Shady Button" logic.
1. Add 1 Task.
2. Verify "Start Focus" is **Enabled**.
3. Delete that Task.
4. Verify "Start Focus" is **Disabled (Shady)** immediately.
5. Add 1 Task again.
6. Verify "Start Focus" is **Enabled** again.
**Expected:** The button state updates instantly every time.
**Broken if:** You can click "Start Focus" with 0 tasks.

## 4. The "Rage Quit" (Persistence Test)
**Goal:** specific app lifecycle behavior.
1. Add 2 tasks.
2. Start Focus.
3. Complete the first task.
4. **Force Close** the app (Swipe it away from recent apps).
5. Re-open the app.
**Expected:** 
   - You should be back at the **Planning Page**.
   - Task 1 should be **Completed** (Checkmark).
   - Task 2 should be **Pending**.
**Broken if:** Task 1 is lost or reset to Pending (Data loss).

## 5. The "Party Pooper" (Animation Stress)
**Goal:** Break the confetti engine.
1. Add 1 Task.
2. Start Focus.
3. Complete it.
4. While the Confetti is falling and screen is shaking, **hit the "Back" button** (Android physical back button or gesture) immediately.
**Expected:** App returns to Planning Page cleanly. Animation stops or cleans up without crashing.
**Broken if:** App crashed with `ObjectDisposedException` or similar.

## 6. The "Sleepy User" (Android Service Test)
**Goal:** Test background survival.
1. Start Focus.
2. Put the phone to sleep (Screen off).
3. Wait 60 seconds (or 5 minutes).
4. Wake up phone.
**Expected:** App is still there, timer/status is correct.
**Broken if:** Android killed the app processes and you lost your content.
