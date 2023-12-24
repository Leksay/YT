using System;
using System.Reactive;
using LauncherClient.Models.Launcher;
using LauncherClient.Models.Launcher.Game;
using ReactiveUI;
using Splat;

namespace LauncherClient.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    #region properties

    internal ReactiveCommand<Unit, Unit> StartGameCommand { get; }
    internal ReactiveCommand<Unit, Unit> OpenLogsCommand { get; }

    internal bool CanStartGame
    {
        get => _canStartGame;
        set => this.RaiseAndSetIfChanged(ref _canStartGame, value);
    }

    internal double Progress
    {
        get => _progress;
        set => this.RaiseAndSetIfChanged(ref _progress, value);
    }

    internal string MessageLabel
    {
        get => _messageLabel;
        set => this.RaiseAndSetIfChanged(ref _messageLabel, value);
    }

    internal bool FilesIsReady
    {
        get => _filesIsReady;
        set => this.RaiseAndSetIfChanged(ref _filesIsReady, value);
    }

    #endregion

    #region attributes

    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    private readonly IGameStarter _gameStarter;

    private double _progress;
    private string _messageLabel;
    private bool _filesIsReady;
    private bool _canStartGame;

    #endregion

    public MainWindowViewModel()
    {
        _gameStarter = Locator.Current.GetService<IGameStarter>();
        if (_gameStarter is null)
        {
            Logger.Error($"Can.t resolve {typeof(IGameStarter)}");
            throw new NullReferenceException($"Can't resolve {typeof(IGameStarter)}");
        }

        var isGameReady = this.WhenAnyValue(
            property1: viewModel => viewModel.FilesIsReady,
            property2: viewModel => viewModel._gameStarter.GameIsStarted,
            selector: (filesIsReady, gameIsStarted) => filesIsReady && !gameIsStarted
        );

        isGameReady.Subscribe(value => CanStartGame = value);

        OpenLogsCommand = ReactiveCommand.Create(NLogUtils.OpenLogFile);
        StartGameCommand = ReactiveCommand.Create(_gameStarter.StartGame);
    }
}