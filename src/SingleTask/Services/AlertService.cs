using SingleTask.Core.Services;

namespace SingleTask.Services;

public class AlertService : IAlertService
{
    public Task ShowAlertAsync(string title, string message, string cancel = "OK")
    {
        return Shell.Current.DisplayAlert(title, message, cancel);
    }

    public Task<bool> ShowConfirmationAsync(string title, string message, string accept = "Yes", string cancel = "No")
    {
        return Shell.Current.DisplayAlert(title, message, accept, cancel);
    }
}
