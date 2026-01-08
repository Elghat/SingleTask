using Xunit;
using Moq;
using SingleTask.Core.ViewModels;
using SingleTask.Core.Services;
using SingleTask.Core.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SingleTask.UnitTests;

public class PerformanceTests
{
    private readonly Mock<IDatabaseService> _mockDatabaseService;
    private readonly Mock<IFocusService> _mockFocusService;
    private readonly Mock<INavigationService> _mockNavigationService;
    private readonly Mock<IAlertService> _mockAlertService;
    private readonly Mock<IDispatcherService> _mockDispatcherService;
    private readonly Mock<INativeFocusService> _mockNativeFocusService;
    private readonly PlanningViewModel _viewModel;

    public PerformanceTests()
    {
        _mockDatabaseService = new Mock<IDatabaseService>();
        _mockFocusService = new Mock<IFocusService>();
        _mockNavigationService = new Mock<INavigationService>();
        _mockAlertService = new Mock<IAlertService>();
        _mockNativeFocusService = new Mock<INativeFocusService>();

        // Setup dispatcher to run immediately
        _mockDispatcherService = new Mock<IDispatcherService>();
        _mockDispatcherService.Setup(d => d.InvokeOnMainThread(It.IsAny<Action>()))
            .Callback<Action>(action => action());

        _viewModel = new PlanningViewModel(
            _mockDatabaseService.Object,
            _mockFocusService.Object,
            _mockNavigationService.Object,
            _mockAlertService.Object,
            _mockDispatcherService.Object,
            _mockNativeFocusService.Object
        );
    }

    [Fact]
    public async Task LoadTasks_With100Items_CompletesSuccessfully()
    {
        // Arrange
        int itemCount = 100;
        var tasks = Enumerable.Range(0, itemCount)
            .Select(i => new TaskItem { Id = i, Title = $"Task {i}", Status = SingleTask.Core.Models.TaskStatus.Pending })
            .ToList();

        _mockDatabaseService.Setup(db => db.GetTasksAsync())
            .ReturnsAsync(tasks);

        // Act
        await _viewModel.LoadTasksCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal(itemCount, _viewModel.Tasks.Count);
        Assert.True(_viewModel.HasPendingTasks);
    }
}
