using System;
using System.Diagnostics;
using System.IO;
using LauncherClient.Models.Launcher.UI;
using NLog;
using ReactiveUI;
using Splat;

namespace LauncherClient.Models.Launcher.Game;

public class GameStarter : ReactiveObject ,IGameStarter
{
    #region attributes

    private readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly string? _gameExePath;
    private readonly IUIDispatcher _uiDispatcher;
    private Process _gameProcess;
    private bool _gameIsStarted;

    #endregion
    
    #region constructors

    public GameStarter()
    {
        _gameExePath = Locator.Current.GetService<AppConfig>()?.GameExePath;
        _uiDispatcher = Locator.Current.GetService<IUIDispatcher>();

        if (string.IsNullOrEmpty(_gameExePath) || _uiDispatcher is null)
            throw new NullReferenceException("Can't resolve services");
    }

    #endregion

    #region IGameStarter

    public bool GameIsStarted
    {
        get => _gameIsStarted;
        private set => _uiDispatcher.ExecuteOnUiThread(() => this.RaiseAndSetIfChanged(ref _gameIsStarted, value));
    }

    
    public void StartGame()
    {
        if (string.IsNullOrEmpty(_gameExePath) || !File.Exists(_gameExePath))
        {
            Logger.Error($"Can't start game. Exe file at path {_gameExePath} is not exists");
            return;
        }

        _gameProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _gameExePath
            },
            EnableRaisingEvents = true
        };

        GameIsStarted = _gameProcess.Start();

        _gameProcess.Exited += (_, _) =>
        {
            _gameProcess = null;
            GameIsStarted = false;
        };
    }

    #endregion
}