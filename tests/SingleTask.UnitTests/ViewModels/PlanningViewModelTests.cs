using Xunit;
using Moq;
using SingleTask.Core.ViewModels;
using SingleTask.Core.Services;
using SingleTask.Core.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SingleTask.UnitTests.ViewModels;

public class PlanningViewModelTests
{
    private readonly Mock<IDatabaseService> _mockDatabaseService;
    private readonly Mock<IFocusService> _mockFocusService;
    private readonly Mock<INavigationService> _mockNavigationService;
    private readonly Mock<IAlertService> _mockAlertService;
    private readonly Mock<IDispatcherService> _mockDispatcherService;
    private readonly Mock<INativeFocusService> _mockNativeFocusService;
    private readonly PlanningViewModel _viewModel;

    public PlanningViewModelTests()
    {
        _mockDatabaseService = new Mock<IDatabaseService>();
        _mockFocusService = new Mock<IFocusService>();
        _mockNavigationService = new Mock<INavigationService>();
        _mockAlertService = new Mock<IAlertService>();
        _mockDispatcherService = new Mock<IDispatcherService>();
        _mockNativeFocusService = new Mock<INativeFocusService>();

        _mockDispatcherService.Setup(d => d.InvokeOnMainThread(It.IsAny<Action>()))
            .Callback<Action>(action => action());

        // Default permission granted
        _mockNativeFocusService.Setup(s => s.CheckNotificationPermissionAsync()).ReturnsAsync(true);

        _viewModel = new PlanningViewModel(
            _mockDatabaseService.Object,
            _mockFocusService.Object,
            _mockNavigationService.Object,
            _mockAlertService.Object,
            _mockDispatcherService.Object,
            _mockNativeFocusService.Object);
    }

    [Fact]
    public async Task AddTask_ValidTitle_AddsToCollectionAndService()
    {
        // Arrange
        _viewModel.NewTaskTitle = "Buy Milk";
        _mockDatabaseService.Setup(db => db.SaveTaskAsync(It.IsAny<TaskItem>())).ReturnsAsync(1);

        // Act
        await _viewModel.AddTaskCommand.ExecuteAsync(null);

        // Assert
        Assert.Single(_viewModel.Tasks);
        _mockDatabaseService.Verify(db => db.SaveTaskAsync(It.IsAny<TaskItem>()), Times.AtLeastOnce);
        Assert.Empty(_viewModel.NewTaskTitle);
    }

    [Fact]
    public async Task SelectTask_ChecksPermissions_BeforeNavigating()
    {
        // Arrange
        var task = new TaskItem { Title = "Test Task" };
        _mockNativeFocusService.Setup(s => s.CheckNotificationPermissionAsync()).ReturnsAsync(false);

        // Act
        await _viewModel.SelectTaskCommand.ExecuteAsync(task);

        // Assert
        // Should request permission
        _mockNativeFocusService.Verify(n => n.RequestNotificationPermissionAsync(), Times.Once);

        // Should start session
        _mockFocusService.Verify(f => f.StartFocusSession(task), Times.Once);
        _mockNavigationService.Verify(n => n.GoToAsync("FocusPage", It.Is<IDictionary<string, object>>(d => d["Task"] == task)), Times.Once);
    }

    [Fact]
    public async Task SelectTask_WithPermission_DoesNotRequest()
    {
        // Arrange
        var task = new TaskItem { Title = "Test Task" };
        _mockNativeFocusService.Setup(s => s.CheckNotificationPermissionAsync()).ReturnsAsync(true);

        // Act
        await _viewModel.SelectTaskCommand.ExecuteAsync(task);

        // Assert
        _mockNativeFocusService.Verify(n => n.RequestNotificationPermissionAsync(), Times.Never);
        _mockFocusService.Verify(f => f.StartFocusSession(task), Times.Once);
    }

    [Fact]
    public async Task StartFocus_StartsFirstOrderedTask()
    {
        // Arrange
        var task2 = new TaskItem { Id = 2, Title = "Task 2", Status = SingleTask.Core.Models.TaskStatus.Pending, Order = 1 };
        var task1 = new TaskItem { Id = 1, Title = "Task 1", Status = SingleTask.Core.Models.TaskStatus.Pending, Order = 0 };

        _viewModel.Tasks.Add(task2);
        _viewModel.Tasks.Add(task1);

        // Act
        await _viewModel.StartFocusCommand.ExecuteAsync(null);

        // Assert
        _mockFocusService.Verify(f => f.StartFocusSession(task1), Times.Once);
        _mockNavigationService.Verify(n => n.GoToAsync("FocusPage", It.IsAny<IDictionary<string, object>>()), Times.Once);
    }
}
