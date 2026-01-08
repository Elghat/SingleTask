using SingleTask.Core.ViewModels;

namespace SingleTask.Views;

public partial class PlanningPage : ContentPage
{
    public PlanningPage(PlanningViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is PlanningViewModel vm)
        {
            try
            {
                await vm.LoadTasksCommand.ExecuteAsync(null);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Unable to load tasks: {ex.Message}", "OK");
            }
        }
    }
}
