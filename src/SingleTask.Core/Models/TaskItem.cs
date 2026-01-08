using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace SingleTask.Core.Models;

public enum TaskStatus
{
    Pending,
    Completed
}

public partial class TaskItem : ObservableObject
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public TaskStatus Status { get; set; } = TaskStatus.Pending;

    public int Order { get; set; }

    [Ignore]
    public bool IsCompleted => Status == TaskStatus.Completed;

    private int _displayIndex;

    [Ignore]
    public int DisplayIndex
    {
        get => _displayIndex;
        set
        {
            if (SetProperty(ref _displayIndex, value))
            {
                OnPropertyChanged(nameof(CanMoveUp));
                OnPropertyChanged(nameof(CanMoveDown));
            }
        }
    }

    private bool _isTop;
    [Ignore]
    public bool IsTop
    {
        get => _isTop;
        set
        {
            if (SetProperty(ref _isTop, value))
            {
                OnPropertyChanged(nameof(CanMoveUp));
            }
        }
    }

    [Ignore]
    public bool CanMoveUp => !IsCompleted && !IsTop;

    [Ignore]
    public bool CanMoveDown => !IsCompleted && DisplayIndex > 1;
}
