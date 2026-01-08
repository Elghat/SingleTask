using SingleTask.Core.ViewModels;
using CommunityToolkit.Mvvm.Messaging;
using SingleTask.Core.Messages;

namespace SingleTask.Views;

public partial class FocusPage : ContentPage
{
    public FocusPage(FocusViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    // Legacy animation logic removed as Celebration is now a separate page.

    protected override bool OnBackButtonPressed()
    {
        // Intercept back button to confirm quit
        if (BindingContext is FocusViewModel vm)
        {
            // Execute the QuitSession command which now handles confirmation
            // We use the command if it can execute
            if (vm.QuitSessionCommand.CanExecute(null))
            {
                vm.QuitSessionCommand.Execute(null);
                return true; // Use 'true' to indicate we handled the back button
            }
        }
        return base.OnBackButtonPressed();
    }
}
