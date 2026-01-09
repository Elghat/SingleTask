using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SingleTask.Core.Models;
using SingleTask.Core.Services;

namespace SingleTask.Core.ViewModels;

public partial class PlanningViewModel : BaseViewModel
{
    private readonly IDatabaseService _databaseService;
    private readonly IFocusService _focusService;
    private readonly INavigationService _navigationService;
    private readonly IAlertService _alertService;
    private readonly IDispatcherService _dispatcherService;
    private readonly INativeFocusService _nativeFocusService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasPendingTasks))]
    private string newTaskTitle = string.Empty;

    private readonly HashSet<string> _approvedDuplicates = new(StringComparer.OrdinalIgnoreCase);

    // SEC-005: Maximum allowed task title length to prevent DoS
    private const int MaxTaskTitleLength = 200;

    // SEC-012: Rate limiting to prevent command flooding
    private DateTime _lastAddTime = DateTime.MinValue;
    private DateTime _lastDeleteTime = DateTime.MinValue;
    private const int CommandCooldownMs = 500;

    public ObservableCollection<TaskItem> Tasks { get; } = new();

    public bool HasPendingTasks => Tasks.Any(t => t.Status == Models.TaskStatus.Pending);

    public PlanningViewModel(IDatabaseService databaseService, IFocusService focusService, INavigationService navigationService, IAlertService alertService, IDispatcherService dispatcherService, INativeFocusService nativeFocusService)
    {
        _databaseService = databaseService;
        _focusService = focusService;
        _navigationService = navigationService;
        _alertService = alertService;
        _dispatcherService = dispatcherService;
        _nativeFocusService = nativeFocusService;
        Title = "Plan Your Day";
    }

    [RelayCommand]
    private async Task LoadTasksAsync()
    {
        IsBusy = true;
        try
        {
            var tasks = await _databaseService.GetTasksAsync();
            // Load by Persisted Order (Ascending: Top=0, Bottom=N)
            // This matches the order saved by Move commands.
            var sortedTasks = tasks.OrderBy(t => t.Order).ToList();

            Tasks.Clear();
            for (int i = 0; i < sortedTasks.Count; i++)
            {
                var task = sortedTasks[i];
                Tasks.Add(task);
            }

            RenumberTasks();
            OnPropertyChanged(nameof(HasPendingTasks));
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task AddTaskAsync()
    {
        // SEC-012: Debounce rapid calls
        if ((DateTime.UtcNow - _lastAddTime).TotalMilliseconds < CommandCooldownMs)
            return;
        _lastAddTime = DateTime.UtcNow;

        if (string.IsNullOrWhiteSpace(NewTaskTitle))
            return;

        var cleanTitle = NewTaskTitle.Trim();

        // SEC-005: Input length validation to prevent memory exhaustion and DB bloat
        if (cleanTitle.Length > MaxTaskTitleLength)
        {
            await _alertService.ShowAlertAsync("Title Too Long",
                $"Task title cannot exceed {MaxTaskTitleLength} characters.", "OK");
            return;
        }

        // Smart Duplicate Check
        bool isDuplicate = Tasks.Any(t => t.Title.Equals(cleanTitle, StringComparison.OrdinalIgnoreCase));

        if (isDuplicate && !_approvedDuplicates.Contains(cleanTitle))
        {
            // Ask user
            bool confirm = await _alertService.ShowConfirmationAsync(
                "Duplicate Task",
                $"'{cleanTitle}' already exists. Do you want to add it anyway?",
                "Yes", "No");

            if (!confirm)
            {
                return; // User cancelled
            }

            // User approved, whitelist this title for future
            _approvedDuplicates.Add(cleanTitle);
        }

        try
        {
            var newTask = new TaskItem
            {
                Title = cleanTitle,
                Status = Models.TaskStatus.Pending,
                Order = 0 // Will be corrected by SaveTaskOrderAsync
            };

            if (await _databaseService.SaveTaskAsync(newTask) > 0)
            {
                _dispatcherService.InvokeOnMainThread(async () =>
                {
                    // Add to Top of Visual List
                    Tasks.Insert(0, newTask);

                    // Normalize Order numbers immediately
                    await SaveTaskOrderAsync();
                    RenumberTasks();

                    NewTaskTitle = string.Empty;
                    OnPropertyChanged(nameof(HasPendingTasks));
                });
            }
        }
        catch (Exception ex)
        {
            await _alertService.ShowAlertAsync("Error", $"Failed to add task: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task DeleteTaskAsync(TaskItem task)
    {
        // SEC-012: Debounce rapid calls
        if ((DateTime.UtcNow - _lastDeleteTime).TotalMilliseconds < CommandCooldownMs)
            return;
        _lastDeleteTime = DateTime.UtcNow;

        if (task == null) return;

        if (await _databaseService.DeleteTaskAsync(task) > 0)
        {
            Tasks.Remove(task);
            await SaveTaskOrderAsync();
            RenumberTasks();
            OnPropertyChanged(nameof(HasPendingTasks));
        }
    }

    [RelayCommand]
    private async Task SelectTaskAsync(TaskItem task)
    {
        if (task == null) return;

        // Check Permissions first
        if (!await _nativeFocusService.CheckNotificationPermissionAsync())
        {
            await _nativeFocusService.RequestNotificationPermissionAsync();
        }

        // Start focus session
        _focusService.StartFocusSession(task);
        _nativeFocusService.TriggerHapticFeedback();

        // Navigate to FocusPage passing the task parameters
        var navigationParameter = new Dictionary<string, object>
        {
            { "Task", task }
        };

        await _navigationService.GoToAsync("FocusPage", navigationParameter);
    }

    [RelayCommand]
    private async Task MoveTaskUp(TaskItem task)
    {
        if (task == null || task.IsCompleted) return;

        int oldIndex = Tasks.IndexOf(task);
        // Find next pending (non-completed) task above
        int targetIndex = -1;
        for (int i = oldIndex - 1; i >= 0; i--)
        {
            if (!Tasks[i].IsCompleted)
            {
                targetIndex = i;
                break;
            }
        }

        if (targetIndex >= 0)
        {
            Tasks.Move(oldIndex, targetIndex);
            await SaveTaskOrderAsync();
            RenumberTasks();
        }
    }

    [RelayCommand]
    private async Task MoveTaskDown(TaskItem task)
    {
        if (task == null || task.IsCompleted) return;

        int oldIndex = Tasks.IndexOf(task);
        // Find next pending (non-completed) task below
        int targetIndex = -1;
        for (int i = oldIndex + 1; i < Tasks.Count; i++)
        {
            if (!Tasks[i].IsCompleted)
            {
                targetIndex = i;
                break;
            }
        }

        if (targetIndex >= 0)
        {
            Tasks.Move(oldIndex, targetIndex);
            await SaveTaskOrderAsync();
            RenumberTasks();
        }
    }

    private async Task SaveTaskOrderAsync()
    {
        // Update Order field for all tasks based on current visual position
        for (int i = 0; i < Tasks.Count; i++)
        {
            Tasks[i].Order = i;
            await _databaseService.SaveTaskAsync(Tasks[i]);
        }
    }

    [RelayCommand]
    private async Task StartFocusAsync()
    {
        try
        {
            // FIFO Execution: Start the oldest pending task (bottom of the list, labeled #1)
            // The user reorders them to choose which one is at the bottom.
            var nextTask = Tasks.LastOrDefault(t => t.Status == Models.TaskStatus.Pending);

            if (nextTask != null)
            {
                await SelectTaskAsync(nextTask);
            }
            else
            {
                await _alertService.ShowAlertAsync("Info", "No pending tasks to start!", "OK");
            }
        }
        catch (Exception ex)
        {
            await _alertService.ShowAlertAsync("Error", $"Failed to start focus: {ex.Message}", "OK");
        }
    }

    private void RenumberTasks()
    {
        // FIFO Numbering: Only pending tasks get numbered (Bottom = 1, Top = N)
        // Completed tasks get DisplayIndex = 0 (hidden)
        var pendingTasks = Tasks.Where(t => !t.IsCompleted).ToList();
        int pendingCount = pendingTasks.Count;

        int pendingIndex = 0;
        for (int i = 0; i < Tasks.Count; i++)
        {
            var task = Tasks[i];
            if (task.IsCompleted)
            {
                task.DisplayIndex = 0; // No number for completed
                task.IsTop = false;
            }
            else
            {
                task.DisplayIndex = pendingCount - pendingIndex;
                task.IsTop = (pendingIndex == 0);
                pendingIndex++;
            }
        }
    }
}