using SingleTask.Core.Models;

namespace SingleTask.Core.Services;

public interface IFocusService
{
    TaskItem? CurrentFocusedTask { get; }
    void StartFocusSession(TaskItem task);
    void UpdateFocusSession(TaskItem task);
    void StopFocusSession();
}
