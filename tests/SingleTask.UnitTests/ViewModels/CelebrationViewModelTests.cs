using Xunit;
using Moq;
using SingleTask.Core.ViewModels;
using SingleTask.Core.Services;
using System.Threading.Tasks;

namespace SingleTask.UnitTests.ViewModels;

public class CelebrationViewModelTests
{
    private readonly Mock<INavigationService> _mockNavigationService;
    private readonly CelebrationViewModel _viewModel;

    public CelebrationViewModelTests()
    {
        _mockNavigationService = new Mock<INavigationService>();
        _viewModel = new CelebrationViewModel(_mockNavigationService.Object);
    }

    [Fact]
    public async Task DismissCommand_NavigatesToPlanningPage_Root()
    {
        // Act
        await _viewModel.DismissCommand.ExecuteAsync(null);

        // Assert
        // Should navigate to absolute route "///PlanningPage" to reset stack
        _mockNavigationService.Verify(n => n.GoToAsync("///PlanningPage"), Times.Once);
    }
}
