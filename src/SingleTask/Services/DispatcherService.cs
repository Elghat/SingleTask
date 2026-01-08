using SingleTask.Core.Services;
using Microsoft.Maui.ApplicationModel;

namespace SingleTask.Services;

public class DispatcherService : IDispatcherService
{
    public void InvokeOnMainThread(Action action)
    {
        MainThread.BeginInvokeOnMainThread(action);
    }
}
