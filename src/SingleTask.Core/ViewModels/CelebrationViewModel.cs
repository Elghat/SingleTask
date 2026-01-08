using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SingleTask.Core.Services;

namespace SingleTask.Core.ViewModels;

public partial class CelebrationViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;

    public CelebrationViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        Title = "Celebration";
    }

    [RelayCommand]
    private async Task DismissAsync()
    {
        // Circular Navigation: Return to Planning Page and reset stack
        await _navigationService.GoToAsync("///PlanningPage");
    }
}
