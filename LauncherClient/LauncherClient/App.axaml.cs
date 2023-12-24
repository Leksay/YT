using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using LauncherClient.Models.Launcher;

namespace LauncherClient;

public partial class App : Application
{
    #region public methods

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            WindowsBootstrapper.BuildWindowsApp();
            desktop.MainWindow = WindowsBootstrapper.BuildViews();
        }

        base.OnFrameworkInitializationCompleted();
    }

    #endregion
}