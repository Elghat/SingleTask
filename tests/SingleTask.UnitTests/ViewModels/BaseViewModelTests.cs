using Xunit;
using SingleTask.Core.ViewModels;
using System.ComponentModel;

namespace SingleTask.UnitTests.ViewModels;

public class BaseViewModelTests
{
    [Fact]
    public void BaseViewModel_ShouldImplementINotifyPropertyChanged()
    {
        // Verify BaseViewModel implements INotifyPropertyChanged
        Assert.True(typeof(INotifyPropertyChanged).IsAssignableFrom(typeof(BaseViewModel)));

        var vm = new BaseViewModel();
        Assert.NotNull(vm);
    }
}
