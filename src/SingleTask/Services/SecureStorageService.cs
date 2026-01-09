using Microsoft.Maui.Storage;
using SingleTask.Core.Services;

namespace SingleTask.Services;

/// <summary>
/// Platform implementation of ISecureStorageService using MAUI SecureStorage.
/// </summary>
public class SecureStorageService : ISecureStorageService
{
    public Task<string?> GetAsync(string key)
    {
        return SecureStorage.Default.GetAsync(key);
    }

    public Task SetAsync(string key, string value)
    {
        return SecureStorage.Default.SetAsync(key, value);
    }
}
