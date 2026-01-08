using CommunityToolkit.Mvvm.Input;

namespace SingleTask.Core.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private int count = 0;

    public MainViewModel()
    {
        Title = "Home";
    }

    [RelayCommand]
    private void IncrementCounter()
    {
        count++;

        if (count == 1)
            CountText = $"Clicked {count} time";
        else
            CountText = $"Clicked {count} times";

        // SemanticScreenReader.Announce(CountText); // Removed for Clean Architecture in Core
    }

    // Using explicit property for the text to demonstrate binding
    // Alternatively we could just bind to a 'Count' property and converter
    // but this mimics the logic in the original MainPage.xaml.cs
    private string countText = "Click me";
    public string CountText
    {
        get => countText;
        set => SetProperty(ref countText, value);
    }
}
