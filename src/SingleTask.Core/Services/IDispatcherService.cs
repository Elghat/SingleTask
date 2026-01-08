namespace SingleTask.Core.Services;

public interface IDispatcherService
{
    void InvokeOnMainThread(Action action);
}
