using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SingleTask.Core.Models;
using SingleTask.Core.Services;

namespace SingleTask.Core.ViewModels;

public partial class FocusViewModel : ObservableObject
{

    private readonly IFocusService _focusService;
    private readonly INavigationService _navigationService;
    private readonly IDatabaseService _databaseService;

    private readonly IAudioService _audioService;
    private readonly IHapticService _hapticService;
    private readonly IAlertService _alertService;
    private readonly INativeFocusService _nativeFocusService;

    [ObservableProperty]
    private TaskItem? _currentTask;

    [ObservableProperty]
    private string _taskTitle = string.Empty;

    [ObservableProperty]
    private string _taskProgressText = string.Empty;

    [ObservableProperty]
    private int _currentTaskIndex;

    [ObservableProperty]
    private int _totalSessionTasks;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotCelebrating))]
    private bool _isCelebrating;

    public bool IsNotCelebrating => !IsCelebrating;

    public FocusViewModel(IFocusService focusService, INavigationService navigationService, IDatabaseService databaseService, IAudioService audioService, IHapticService hapticService, IAlertService alertService, INativeFocusService nativeFocusService)
    {
        _focusService = focusService;
        _navigationService = navigationService;
        _databaseService = databaseService;
        _audioService = audioService;
        _hapticService = hapticService;
        _alertService = alertService;
        _nativeFocusService = nativeFocusService;

        if (_focusService.CurrentFocusedTask != null)
        {
            // FR-001: Fire-and-forget with proper exception handling
            _ = InitializeAsync(_focusService.CurrentFocusedTask);
        }
    }

    private async Task UpdateTaskProgressAsync()
    {
        var tasks = await _databaseService.GetTasksAsync();
        TotalSessionTasks = tasks.Count;

        var completedCount = tasks.Count(t => t.Status == SingleTask.Core.Models.TaskStatus.Completed);

        // Progress: (Completed + 1) of Total
        // If current task is pending, it's the next one to complete
        if (CurrentTask != null && CurrentTask.Status == SingleTask.Core.Models.TaskStatus.Pending)
        {
            CurrentTaskIndex = completedCount + 1;
            TaskProgressText = $"Task {CurrentTaskIndex} of {TotalSessionTasks}";
        }
        else if (TotalSessionTasks > 0 && completedCount == TotalSessionTasks)
        {
            TaskProgressText = "All Done!";
        }
    }

    /// <summary>
    /// FR-001: Async initialization with proper exception handling.
    /// Converted from async void to async Task to prevent unhandled exceptions.
    /// </summary>
    public async Task InitializeAsync(TaskItem task)
    {
        try
        {
            CurrentTask = task;
            TaskTitle = task.Title;
            IsCelebrating = false;
            await UpdateTaskProgressAsync();
        }
        catch (Exception ex)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[FocusViewModel.InitializeAsync] Error: {ex.Message}");
#endif
        }
    }

    [RelayCommand]
    private async Task CompleteTask()
    {
        if (CurrentTask != null)
        {
            // 1. Mark current as completed and save
            CurrentTask.Status = SingleTask.Core.Models.TaskStatus.Completed;
            await _databaseService.SaveTaskAsync(CurrentTask);

            // 2. Find next pending task
            // Execution Policy: Bottom-Up (FIFO by User Custom Order)
            var tasks = await _databaseService.GetTasksAsync();
            var nextTask = tasks.Where(t => t.Status == SingleTask.Core.Models.TaskStatus.Pending)
                                .OrderByDescending(t => t.Order) // Bottom-Up Priority
                                .FirstOrDefault();

            if (nextTask != null)
            {
                // 3a. Transition to next task
                _nativeFocusService.TriggerHapticFeedback();
                await _nativeFocusService.PlaySuccessSoundAsync();

                await InitializeAsync(nextTask);
                _focusService.UpdateFocusSession(nextTask);
            }
            else
            {
                // 3b. All done! Trigger Celebration
                _focusService.StopFocusSession();

                // Trigger Native Feedback
                _nativeFocusService.TriggerHapticFeedback();
                await _nativeFocusService.PlaySuccessSoundAsync();

                await _navigationService.GoToAsync("../CelebrationPage");
            }
        }
    }

    [RelayCommand]
    private async Task DeferTask()
    {
        if (CurrentTask == null) return;

        // 1. Calculate new Order (Move to TOP of list = Lowest Order - 1)
        var tasks = await _databaseService.GetTasksAsync();
        var minOrder = tasks.Any() ? tasks.Min(t => t.Order) : 0;

        CurrentTask.Order = minOrder - 1;
        await _databaseService.SaveTaskAsync(CurrentTask);

        // 2. Find next pending task (Bottom-Up)
        var nextTask = tasks.Where(t => t.Id != CurrentTask.Id && t.Status == SingleTask.Core.Models.TaskStatus.Pending)
                            .OrderByDescending(t => t.Order) // Next highest order
                            .FirstOrDefault();

        if (nextTask != null)
        {
            _nativeFocusService.TriggerHapticFeedback();
            await InitializeAsync(nextTask);
            _focusService.UpdateFocusSession(nextTask);
        }
        else
        {
            // Only one task? It stays.
            await UpdateTaskProgressAsync();
        }
    }

    [RelayCommand]
    private async Task QuitSession()
    {
        var confirmed = await _alertService.ShowConfirmationAsync("Stop Session?", "Are you sure you want to quit the current focus session?");
        if (confirmed)
        {
            _focusService.StopFocusSession();
            _nativeFocusService.StopSession(); // Ensure native service stops
            _nativeFocusService.TriggerHapticFeedback();

            await _navigationService.GoBackAsync();
        }
    }

    [RelayCommand]
    private Task ExitFocus() => QuitSession();
}