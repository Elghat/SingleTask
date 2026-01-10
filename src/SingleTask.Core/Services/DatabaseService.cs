using SQLite;
using SingleTask.Core.Models;

namespace SingleTask.Core.Services;

/// <summary>
/// SEC-002: Database service with SQLCipher encryption.
/// Automatically migrates existing unencrypted databases to encrypted format.
/// </summary>
public class DatabaseService : IDatabaseService, IAsyncDisposable
{
    private SQLiteAsyncConnection? _database;
    private readonly string _dbPath;
    private readonly ISecureStorageService _secureStorage;
    private const string DbKeyStorageKey = "singletask_db_key";

    public DatabaseService(string dbPath, ISecureStorageService secureStorage)
    {
        _dbPath = dbPath;
        _secureStorage = secureStorage;
    }

    private async Task InitAsync()
    {
        if (_database != null)
            return;

        // SEC-002: Get or create encryption key
        var encryptionKey = await GetOrCreateEncryptionKeyAsync();

        // Check if we need to migrate from unencrypted DB
        MigrateUnencryptedDatabase(encryptionKey);

        // Create encrypted connection
        var options = new SQLiteConnectionString(_dbPath, true, key: encryptionKey);
        _database = new SQLiteAsyncConnection(options);

        await _database.CreateTableAsync<TaskItem>();
    }

    /// <summary>
    /// Get or create a device-unique encryption key stored in SecureStorage.
    /// </summary>
    private async Task<string> GetOrCreateEncryptionKeyAsync()
    {
        var key = await _secureStorage.GetAsync(DbKeyStorageKey);
        if (string.IsNullOrEmpty(key))
        {
            // Generate a new 256-bit key (64 hex chars)
            key = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
            await _secureStorage.SetAsync(DbKeyStorageKey, key);
        }
        return key;
    }

    /// <summary>
    /// Migrate an existing unencrypted database to encrypted format.
    /// </summary>
    private void MigrateUnencryptedDatabase(string encryptionKey)
    {
        // Check if DB file exists
        if (!File.Exists(_dbPath))
            return;

        var backupPath = _dbPath + ".unencrypted_backup";
        var tempEncryptedPath = _dbPath + ".encrypted_temp";

        // First, check if the database is already encrypted
        try
        {
            var testOptions = new SQLiteConnectionString(_dbPath, true, key: encryptionKey);
            using var testDb = new SQLiteConnection(testOptions);
            testDb.Execute("SELECT count(*) FROM sqlite_master;");
            // Success - already encrypted, no migration needed
            return;
        }
        catch (Exception ex)
        {
            // Database is unencrypted or uses different key, need to migrate
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[DatabaseService.MigrateUnencryptedDatabase] Check encrypted: {ex.Message}");
#endif
        }

        // Backup unencrypted DB
        File.Copy(_dbPath, backupPath, overwrite: true);

        try
        {
            // Read all data from unencrypted DB
            using var unencryptedDb = new SQLiteConnection(_dbPath);

            // Try to read tables - if this fails, DB might be corrupted
            List<TaskItem> tasks;
            try
            {
                unencryptedDb.CreateTable<TaskItem>();
                tasks = unencryptedDb.Table<TaskItem>().ToList();
            }
            catch (Exception ex)
            {
                // Failed to read - might be encrypted with different key or corrupted
                // Start fresh
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"[DatabaseService.MigrateUnencryptedDatabase] Read tables: {ex.Message}");
#endif
                unencryptedDb.Close();
                File.Delete(_dbPath);
                return;
            }
            unencryptedDb.Close();

            // Create new encrypted DB
            var encOptions = new SQLiteConnectionString(tempEncryptedPath, true, key: encryptionKey);
            using var encryptedDb = new SQLiteConnection(encOptions);
            encryptedDb.CreateTable<TaskItem>();

            // Copy data
            foreach (var task in tasks)
                encryptedDb.Insert(task);
            encryptedDb.Close();

            // Replace old DB with encrypted one
            File.Delete(_dbPath);
            File.Move(tempEncryptedPath, _dbPath);

            // Note: backup is preserved at backupPath for safety
        }
        catch (Exception ex)
        {
            // Migration failed - cleanup temp file if exists
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[DatabaseService.MigrateUnencryptedDatabase] Migration failed: {ex.Message}");
#endif
            if (File.Exists(tempEncryptedPath))
                File.Delete(tempEncryptedPath);

            // Restore from backup and start fresh
            if (File.Exists(backupPath))
            {
                File.Delete(_dbPath);
                // Don't restore - just start fresh with encrypted DB
            }
        }
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
