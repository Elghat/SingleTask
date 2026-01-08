using Xunit;
using Moq;
using SingleTask.Core.ViewModels;
using SingleTask.Core.Services;
using SingleTask.Core.Models;
using System.Threading.Tasks;

namespace SingleTask.UnitTests.ViewModels;

public class FocusViewModelTests
{
    private readonly Mock<IFocusService> _mockFocusService;
    private readonly Mock<INavigationService> _mockNavigationService;
    private readonly Mock<IDatabaseService> _mockDatabaseService;
    private readonly Mock<IAudioService> _mockAudioService;
    private readonly Mock<IHapticService> _mockHapticService;
    private readonly Mock<IAlertService> _mockAlertService;
    private readonly Mock<INativeFocusService> _mockNativeFocusService;
    private readonly FocusViewModel _viewModel;

    public FocusViewModelTests()
    {
        _mockFocusService = new Mock<IFocusService>();
        _mockNavigationService = new Mock<INavigationService>();
        _mockDatabaseService = new Mock<IDatabaseService>();
        _mockAudioService = new Mock<IAudioService>();
        _mockHapticService = new Mock<IHapticService>();
        _mockAlertService = new Mock<IAlertService>();
        _mockNativeFocusService = new Mock<INativeFocusService>();

        _viewModel = new FocusViewModel(
            _mockFocusService.Object,
            _mockNavigationService.Object,
            _mockDatabaseService.Object,
            _mockAudioService.Object,
            _mockHapticService.Object,
            _mockAlertService.Object,
            _mockNativeFocusService.Object);
    }

    [Fact]
    public void Initialize_SetsProperties()
    {
        // Arrange
        var task = new TaskItem { Title = "Focus Task" };
        _mockDatabaseService.Setup(db => db.GetTasksAsync())
            .ReturnsAsync(new System.Collections.Generic.List<TaskItem> { task });

        // Act
        _viewModel.Initialize(task);

        // Assert
        Assert.Equal("Focus Task", _viewModel.TaskTitle);
        Assert.Equal(task, _viewModel.CurrentTask);
    }

    [Fact]
    public async Task CompleteTask_WithNextTask_UpdatesToNext()
    {
        // Arrange
        var currentTask = new TaskItem { Id = 1, Title = "Current", Status = SingleTask.Core.Models.TaskStatus.Pending, Order = 0 };
        var nextTask = new TaskItem { Id = 2, Title = "Next", Status = SingleTask.Core.Models.TaskStatus.Pending, Order = 1 };

        _mockDatabaseService.Setup(db => db.GetTasksAsync())
            .ReturnsAsync(new System.Collections.Generic.List<TaskItem> { currentTask, nextTask });

        _viewModel.Initialize(currentTask);

        // Act
        await _viewModel.CompleteTaskCommand.ExecuteAsync(null);

        // Assert
        // 1. Current marked complete and saved
        Assert.Equal(SingleTask.Core.Models.TaskStatus.Completed, currentTask.Status);
        _mockDatabaseService.Verify(db => db.SaveTaskAsync(currentTask), Times.Once);

        // 2. ViewModel switched to Next
        Assert.Equal(nextTask, _viewModel.CurrentTask);
        Assert.Equal("Next", _viewModel.TaskTitle);

        // 3. Service updated
        _mockFocusService.Verify(s => s.UpdateFocusSession(nextTask), Times.Once);

        // 4. Native Feedback Triggered
        _mockNativeFocusService.Verify(n => n.PlaySuccessSoundAsync(), Times.Once);
        _mockNativeFocusService.Verify(n => n.TriggerHapticFeedback(), Times.Once);

        // 5. Did NOT stop or go to Celebration yet
        _mockFocusService.Verify(s => s.StopFocusSession(), Times.Never);
        _mockNavigationService.Verify(n => n.GoToAsync("CelebrationPage"), Times.Never);
    }

    [Fact]
    public async Task CompleteTask_LastTask_NavigatesToCelebration()
    {
        // Arrange
        var currentTask = new TaskItem { Id = 1, Title = "Last One", Status = SingleTask.Core.Models.TaskStatus.Pending, Order = 0 };

        _mockDatabaseService.Setup(db => db.GetTasksAsync())
            .ReturnsAsync(new System.Collections.Generic.List<TaskItem> { currentTask }); // No others pending

        _viewModel.Initialize(currentTask);

        // Act
        await _viewModel.CompleteTaskCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal(SingleTask.Core.Models.TaskStatus.Completed, currentTask.Status);

        // Should stop session
        _mockFocusService.Verify(s => s.StopFocusSession(), Times.Once);

        // Native Feedback
        _mockNativeFocusService.Verify(n => n.PlaySuccessSoundAsync(), Times.Once);
        _mockNativeFocusService.Verify(n => n.TriggerHapticFeedback(), Times.Once);

        // Should go to Celebration
        _mockNavigationService.Verify(n => n.GoToAsync("../CelebrationPage"), Times.Once);
    }
}
