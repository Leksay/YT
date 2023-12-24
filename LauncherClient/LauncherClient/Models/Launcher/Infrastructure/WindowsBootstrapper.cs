using LauncherClient.Models.Launcher.Game;
using LauncherClient.Models.Launcher.UI;
using LauncherClient.ViewModels;
using LauncherClient.Views;
using Shared.Hash;
using Splat;

namespace LauncherClient.Models.Launcher;

public static class WindowsBootstrapper
{
    #region public methods

    public static void BuildWindowsApp()
    {
        NLogUtils.SetConfig();

        RegisterAs<AppConfig, AppConfig>(AppConfig.GetConfig());
        RegisterAs<Crc32HashCalculator, IHashCalculator>(new Crc32HashCalculator());
        RegisterAs<MockApiHandler, IApiHandler>(new MockApiHandler());
        RegisterAs<AvaloniaUiDispatcher, IUIDispatcher>(new AvaloniaUiDispatcher());
        RegisterAs<GameStarter, IGameStarter>(new GameStarter());
        RegisterAs<MainWindowViewModel, MainWindowViewModel>(new MainWindowViewModel());

        RegisterAs<UpdateHandler, UpdateHandler>(new UpdateHandler());
    }

    #endregion

    #region service methods

    public static MainWindow BuildViews()
    {
        var mainViewModel = Locator.Current.GetService<MainWindowViewModel>();

        var mainView = new MainWindow
        {
            DataContext = mainViewModel
        };

        return mainView;
    }

    private static void RegisterAs<TInstance, TInterface>(TInstance instance) where TInstance : class, TInterface
    {
        Locator.CurrentMutable.Register(() => instance, typeof(TInterface));
    }

    #endregion
}