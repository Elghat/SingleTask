using Xunit;
using SingleTask.Core.Services;
using SingleTask.Core.Models;
using System.IO;

namespace SingleTask.UnitTests.Services;

public class DatabaseServiceTests
{
    [Fact]
    public async Task DatabaseService_ShouldInitializedAndSaveItem()
    {
        var dbPath = Path.GetTempFileName();
        var service = new DatabaseService(dbPath);

        var item = new TestEntity { Name = "Test Item", CreatedAt = DateTime.UtcNow };
        var result = await service.SaveItemAsync(item);

        Assert.Equal(1, result); // 1 row affected

        var items = await service.GetItemsAsync();
        Assert.NotEmpty(items);
        Assert.Equal("Test Item", items[0].Name);

        // Cleanup
        await service.CloseAsync();
        if (File.Exists(dbPath))
            File.Delete(dbPath);
    }
}
