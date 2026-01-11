using Xunit;
using SingleTask.Core.Services;
using SingleTask.Core.Models;
using System.IO;

namespace SingleTask.UnitTests.Services;

public class DatabaseServiceTests
{
    [Fact]
    public async Task DatabaseService_ShouldInitializeAndSaveTask()
    {
        var dbPath = Path.GetTempFileName();
        var service = new DatabaseService(dbPath);

        var task = new TaskItem { Title = "Test Task", Order = 0 };
        var result = await service.SaveTaskAsync(task);

        Assert.Equal(1, result); // 1 row affected

        var tasks = await service.GetTasksAsync();
        Assert.NotEmpty(tasks);
        Assert.Equal("Test Task", tasks[0].Title);

        // Cleanup
        await service.CloseAsync();
        if (File.Exists(dbPath))
            File.Delete(dbPath);
    }

    /// <summary>
    /// FR-004: Test batch save operation for reorder performance.
    /// </summary>
    [Fact]
    public async Task DatabaseService_SaveTasksAsync_ShouldBatchSave()
    {
        var dbPath = Path.GetTempFileName();
        var service = new DatabaseService(dbPath);

        // Create initial tasks
        var task1 = new TaskItem { Title = "Task 1", Order = 0 };
        var task2 = new TaskItem { Title = "Task 2", Order = 1 };
        var task3 = new TaskItem { Title = "Task 3", Order = 2 };

        await service.SaveTaskAsync(task1);
        await service.SaveTaskAsync(task2);
        await service.SaveTaskAsync(task3);

        // Reorder: reverse the order
        task1.Order = 2;
        task2.Order = 1;
        task3.Order = 0;

        // Batch save
        await service.SaveTasksAsync(new[] { task1, task2, task3 });

        // Verify order was saved
        var tasks = await service.GetTasksAsync();
        Assert.Equal(3, tasks.Count);
        Assert.Equal("Task 3", tasks[0].Title); // Order 0
        Assert.Equal("Task 2", tasks[1].Title); // Order 1
        Assert.Equal("Task 1", tasks[2].Title); // Order 2

        // Cleanup
        await service.CloseAsync();
        if (File.Exists(dbPath))
            File.Delete(dbPath);
    }
}
