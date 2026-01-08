# Data Model: Infrastructure Patterns

**Feature**: `2-maui-infra-setup`

## Base Classes

### `BaseViewModel`
- Inherits: `ObservableObject` (CommunityToolkit.Mvvm)
- Properties:
  - `IsBusy` (bool)
  - `Title` (string)

## Validation Entities (for Setup Verification)

### `TestEntity`
- Purpose: Verify SQLite connection.
- Table: `TestEntity`
- Columns:
  - `Id` (Primary Key, AutoIncrement, int)
  - `Name` (string)
  - `CreatedAt` (DateTime)

## Database Schema (Planned)
- No core domain entities yet. This feature only sets up the *capability* to store entities.
