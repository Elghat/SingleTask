using SQLite;

namespace SingleTask.Core.Models;

public class TestEntity
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
