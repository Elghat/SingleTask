using Xunit;
using SingleTask.Core.Services;
using SingleTask.Core.Models;
using System.IO;

namespace SingleTask.UnitTests.Services;

/// <summary>
/// Mock implementation of ISecureStorageService for unit testing.
/// </summary>
public class MockSecureStorageService : ISecureStorageService
{
    private readonly Dictionary<string, string> _storage = new();

    public Task<string?> GetAsync(string key)
    {
        _storage.TryGetValue(key, out var value);
        return Task.FromResult(value);
    }

    public Task SetAsync(string key, string value)
    {
        _storage[key] = value;
        return Task.CompletedTask;
    }
}

public class DatabaseServiceTests
{
    [Fact]
    public async Task DatabaseService_ShouldInitializedAndSaveItem()
    {
        var dbPath = Path.GetTempFileName();
        var secureStorage = new MockSecureStorageService();
        var service = new DatabaseService(dbPath, secureStorage);

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
