using SingleTask.Core.Models;

namespace SingleTask.Core.Services;

/// <summary>
/// Database service interface for task persistence.
/// SEC-002: Uses SQLCipher encryption.
/// FR-014: Implements IAsyncDisposable for proper cleanup.
/// </summary>
public interface IDatabaseService : IAsyncDisposable
{
    Task CloseAsync();

    // TaskItem Operations
    Task<List<TaskItem>> GetTasksAsync();
    Task<int> SaveTaskAsync(TaskItem item);
    Task<int> DeleteTaskAsync(TaskItem item);

    /// <summary>
    /// Batch save multiple tasks in a single transaction.
    /// FR-004: Performance optimization for reordering operations.
    /// </summary>
    Task SaveTasksAsync(IEnumerable<TaskItem> tasks);
}
