using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using NLog;
using NLog.Targets;

namespace LauncherClient.Models.Launcher;

public static class NLogUtils
{
    #region constants

    private const string DefaultOpenLogApplication = "notepad";
    private const string DateTimeFormat = "yyyy-dd-M--HH-mm-ss";
    private static readonly string TimeRelativeLogFile = Path.Combine("Logs", $"{DateTime.Now.ToString(DateTimeFormat)}_logs.txt");

    #endregion

    #region public methods

    public static void SetConfig()
    { 
        LogManager.Setup().LoadConfiguration(builder =>
        {
            builder.ForLogger().FilterMinLevel(LogLevel.Info).WriteToConsole();
            builder.ForLogger().FilterMinLevel(LogLevel.Debug).WriteToFile(fileName: TimeRelativeLogFile);
        });
    }

    public static void OpenLogFile()
    {
        var fileTarget = LogManager.Configuration?.AllTargets.OfType<FileTarget>();
        var localPath = fileTarget?.FirstOrDefault()?.FileName?.Render(LogEventInfo.CreateNullEvent());
        var path = Path.Combine(AppContext.BaseDirectory, localPath);

        Thread.Sleep(500);

        try
        {
            if (!string.IsNullOrEmpty(path) && File.Exists(path)) 
                Process.Start(DefaultOpenLogApplication, $"\"{path}\"");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    #endregion
}