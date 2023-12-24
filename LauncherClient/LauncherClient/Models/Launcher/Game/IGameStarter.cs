namespace LauncherClient.Models.Launcher.Game;

public interface IGameStarter
{
    void StartGame();

    bool GameIsStarted { get; }
}