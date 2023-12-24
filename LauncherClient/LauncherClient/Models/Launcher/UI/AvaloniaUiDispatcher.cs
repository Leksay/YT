using System;
using Avalonia.Threading;

namespace LauncherClient.Models.Launcher.UI;

public class AvaloniaUiDispatcher : IUIDispatcher
{
    #region IUIDispatcher

    public void ExecuteOnUiThread(Action action) => Dispatcher.UIThread.Invoke(action);

    #endregion
}