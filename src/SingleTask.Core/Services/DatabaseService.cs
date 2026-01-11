using SQLite;
using SingleTask.Core.Models;

namespace SingleTask.Core.Services;

/// <summary>
/// Simple database service using plain SQLite (no encryption).
/// Provides reliable task persistence without SecureStorage key dependency.
/// </summary>
public class DatabaseService : IDatabaseService, IAsyncDisposable
{
    private SQLiteAsyncConnection? _database;
    private readonly string _dbPath;

    public DatabaseService(string dbPath)
    {
        _dbPath = dbPath;
    }

    private async Task InitAsync()
    {
        if (_database != null)
            return;

        // Simple unencrypted SQLite connection
        _database = new SQLiteAsyncConnection(_dbPath);
        await _database.CreateTableAsync<TaskItem>();
    }

    public async Task<List<TaskItem>> GetTasksAsync()
    {
        await InitAsync();
        return await _database!.Table<TaskItem>().OrderBy(t => t.Order).ToListAsync();
    }

    public async Task<int> SaveTaskAsync(TaskItem item)
    {
        await InitAsync();
        if (item.Id != 0)
            return await _database!.UpdateAsync(item);
        else
            return await _database!.InsertAsync(item);
    }

    public async Task<int> DeleteTaskAsync(TaskItem item)
    {
        await InitAsync();
        return await _database!.DeleteAsync(item);
    }

    /// <summary>
    /// FR-004: Batch save multiple tasks in a single transaction for performance.
    /// </summary>
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

    public async Task CloseAsync()
    {
        if (_database != null)
        {
            await _database.CloseAsync();
            _database = null;
        }
    }

    /// <summary>
    /// FR-014: Implement IAsyncDisposable for proper resource cleanup.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        await CloseAsync();
    }
}
