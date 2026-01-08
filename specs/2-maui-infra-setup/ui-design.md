# UI Design Specification: Paper & Ink Theme

## 1. Color Palette (The Truth)
Do not guess colors from images. Use these exact HEX codes.

* **Background (Page):** `#F3F4F6` (Light Grey paper)
* **Surface (Cards, Input):** `#FFFFFF` (Pure White)
* **Primary Action (Ink):** `#111827` (Deep Black - Buttons, Main Text, Checkmarks)
* **Secondary Text (Subtitles, Links):** `#6B7280` (Medium Grey)
* **OnPrimary Text:** `#FFFFFF` (White text on Black buttons)

## 2. Component Styling Rules (Based on Mockups)

### Buttons (Main Action - "DONE", "GO")
* Shape: Perfectly Circular.
* Color: `Primary Action` background.
* Text: Bold, Uppercase, `OnPrimary Text` color.
* Placement: Centered, prominent.

### Input Fields & Cards (Task History)
* Shape: Rounded Rectangles (CornerRadius approx 12-16dp).
* Background: `Surface` color.
* Border: Very subtle thin grey border or none if using shadow.
* Shadow: Subtle, soft shadow to lift it from the background.

### Typography
* **Main Task Title:** Very Large, Bold, Centered, `Primary Action` color.
* **Standard Text:** `Primary Action` color.
* **Timestamps/Links:** Smaller font, `Secondary Text` color.