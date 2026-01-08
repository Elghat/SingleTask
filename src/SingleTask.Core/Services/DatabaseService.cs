using SQLite;
using SingleTask.Core.Models;

namespace SingleTask.Core.Services;

public class DatabaseService : IDatabaseService
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

        _database = new SQLiteAsyncConnection(_dbPath);
        await _database.CreateTableAsync<TestEntity>();
        await _database.CreateTableAsync<TaskItem>();
    }

    public async Task<List<TestEntity>> GetItemsAsync()
    {
        await InitAsync();
        return await _database!.Table<TestEntity>().ToListAsync();
    }

    public async Task<int> SaveItemAsync(TestEntity item)
    {
        await InitAsync();
        if (item.Id != 0)
            return await _database!.UpdateAsync(item);
        else
            return await _database!.InsertAsync(item);
    }

    public async Task<int> DeleteItemAsync(TestEntity item)
    {
        await InitAsync();
        return await _database!.DeleteAsync(item);
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

    public async Task CloseAsync()
    {
        if (_database != null)
        {
            await _database.CloseAsync();
            _database = null;
        }
    }
}
