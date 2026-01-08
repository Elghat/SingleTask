using SingleTask.Core.Models;

namespace SingleTask.Core.Services;

public interface IDatabaseService
{
    Task<List<TestEntity>> GetItemsAsync();
    Task<int> SaveItemAsync(TestEntity item);
    Task<int> DeleteItemAsync(TestEntity item);
    Task CloseAsync();

    // TaskItem Operations
    Task<List<TaskItem>> GetTasksAsync();
    Task<int> SaveTaskAsync(TaskItem item);
    Task<int> DeleteTaskAsync(TaskItem item);
}
