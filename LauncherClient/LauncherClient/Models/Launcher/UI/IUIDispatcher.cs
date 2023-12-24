using System;

namespace LauncherClient.Models.Launcher.UI;

public interface IUIDispatcher
{
    public void ExecuteOnUiThread(Action action);
}