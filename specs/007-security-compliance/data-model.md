# Data Model: Security Compliance

**Feature**: 007-security-compliance

## 1. Schema Changes

### Existing Entities (Unchanged Structure)
The `TaskItem` and `TestEntity` structures remain the same. The change is at the **storage layer** (encryption).

```csharp
public class TaskItem
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    // ... other fields
}
```

## 2. New Services

### IEncryptionService (New)

Responsible for managing the database encryption key.

```csharp
public interface IEncryptionService
{
    /// <summary>
    /// Retrieves the existing database key or generates a new one securely.
    /// </summary>
    /// <returns>Base64 encoded key or raw bytes string</returns>
    Task<string> GetDatabaseKeyAsync();
}
```

**Implementation Strategy**:
- Use `SecureStorage.GetAsync("db_key")`.
- If null, generate `Guid.NewGuid().ToString()`, store it, and return it.

### IDatabaseService (Modified)

The public interface remains `IDatabaseService`, but the implementation `DatabaseService` changes.

```csharp
public class DatabaseService : IDatabaseService
{
    public DatabaseService(string dbPath, IEncryptionService encryptionService) { ... }
    
    // InitAsync logic handles the key injection
}
```

## 3. Data Migration Flow

On application startup (`InitAsync`):

1.  **Get Key**: `var key = await _encryptionService.GetDatabaseKeyAsync();`
2.  **Attempt Connect**: Try to open `SQLiteAsyncConnection` with `key`.
3.  **Handle Failure (Plaintext Check)**:
    - If connection fails (exception), check if the file exists and might be plaintext.
    - *Migration*:
        - Rename `SingleTask.db3` to `SingleTask_temp.db3`.
        - Open `SingleTask_temp.db3` (No Key).
        - Open `SingleTask.db3` (With Key).
        - Copy all records from Temp to New.
        - Delete Temp.

## 4. Input Validation Rules (ViewModel Layer)

**PlanningViewModel**:

| Field | Rule | Error Message |
|-------|------|---------------|
| `NewTaskTitle` | Max Length 500 | "Task title is too long (max 500 chars)." |
| `AddTask` Action | Rate Limit (500ms) | N/A (Ignore/Debounce) |

## 5. Build Configuration (CSPROJ)

**Release Profile**:
- `AndroidKeyStore`: True
- `AndroidSigningStorePass`: From Environment
- `AndroidLinkMode`: SdkOnly
- `PublishTrimmed`: True
