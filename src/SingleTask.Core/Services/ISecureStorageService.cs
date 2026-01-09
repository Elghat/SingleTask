namespace SingleTask.Core.Services;

/// <summary>
/// Abstraction for secure key-value storage.
/// Used for storing the database encryption key.
/// </summary>
public interface ISecureStorageService
{
    Task<string?> GetAsync(string key);
    Task SetAsync(string key, string value);
}
