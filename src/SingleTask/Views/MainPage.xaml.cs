namespace SingleTask.Views;

public partial class MainPage : ContentPage
{
    public MainPage(SingleTask.Core.ViewModels.MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
