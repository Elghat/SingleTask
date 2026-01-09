using Microsoft.Extensions.Logging;
using SingleTask.Views;

namespace SingleTask;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("MPLUSRounded1c-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("MPLUSRounded1c-Regular.ttf", "OpenSansSemibold");

                // Warm Paper UI Aliases
                fonts.AddFont("MPLUSRounded1c-Regular.ttf", "Inter");
                fonts.AddFont("MPLUSRounded1c-Regular.ttf", "InterBold");
                fonts.AddFont("MPLUSRounded1c-Regular.ttf", "PlayfairDisplay");
                fonts.AddFont("MPLUSRounded1c-Regular.ttf", "PlayfairDisplayBold");

                // Fallback for missing icon font (User provided text font instead of icons)
                fonts.AddFont("MPLUSRounded1c-Regular.ttf", "MaterialSymbols");
            });

        builder.Services.AddSingleton<SingleTask.Core.ViewModels.MainViewModel>();
        builder.Services.AddSingleton<SingleTask.Views.MainPage>();

        builder.Services.AddSingleton<SingleTask.Core.ViewModels.PlanningViewModel>();
        builder.Services.AddSingleton<SingleTask.Views.PlanningPage>();

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "SingleTask.db3");
        builder.Services.AddSingleton<SingleTask.Core.Services.ISecureStorageService, SingleTask.Services.SecureStorageService>();
        builder.Services.AddSingleton<SingleTask.Core.Services.IDatabaseService>(s =>
            new SingleTask.Core.Services.DatabaseService(dbPath, s.GetRequiredService<SingleTask.Core.Services.ISecureStorageService>()));

        builder.Services.AddSingleton<SingleTask.Core.Services.INavigationService, SingleTask.Services.NavigationService>();
        builder.Services.AddSingleton<SingleTask.Core.Services.IAlertService, SingleTask.Services.AlertService>();
        builder.Services.AddSingleton<SingleTask.Core.Services.IDispatcherService, SingleTask.Services.DispatcherService>();

        // Focus Features
        builder.Services.AddTransient<SingleTask.Core.ViewModels.FocusViewModel>();
        builder.Services.AddTransient<FocusPage>();

        builder.Services.AddTransient<SingleTask.Core.ViewModels.CelebrationViewModel>();
        builder.Services.AddTransient<SingleTask.Views.CelebrationPage>();

        builder.Services.AddSingleton<SingleTask.Core.Services.IHapticService, SingleTask.Services.HapticService>();

#if ANDROID
        // Register concrete service
        builder.Services.AddSingleton<SingleTask.Platforms.Android.Services.AndroidFocusService>();
        // Forward interfaces to concrete service
        builder.Services.AddSingleton<SingleTask.Core.Services.IFocusService>(s => s.GetRequiredService<SingleTask.Platforms.Android.Services.AndroidFocusService>());
        builder.Services.AddSingleton<SingleTask.Core.Services.INativeFocusService>(s => s.GetRequiredService<SingleTask.Platforms.Android.Services.AndroidFocusService>());

        builder.Services.AddSingleton<SingleTask.Core.Services.IAudioService, SingleTask.Platforms.Android.Services.AudioService>();
#else
        builder.Services.AddSingleton<SingleTask.Core.Services.INativeFocusService, SingleTask.Core.Services.NativeFocusServiceStub>();
#endif




#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
