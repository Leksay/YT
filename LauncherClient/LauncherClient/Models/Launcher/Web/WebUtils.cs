using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace LauncherClient.Models.Launcher;

public static class WebUtils
{
    #region attributes

    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();    

    #endregion

    #region public methods

    public static async Task<bool> CheckForConnection(string url, int timeoutMs = 5000)
    {
        Uri uri = new Uri(url);
        if (!uri.IsWellFormedOriginalString())
        {
            Logger.Info("Uri {0} in not formated", uri);
            return false;
        }

        using var client = new HttpClient();
        client.Timeout = new TimeSpan(0, 0, 0, timeoutMs);

        try
        {
            using var response = await client.GetAsync(url);
            return response.IsSuccessStatusCode;
        }
        catch (Exception e)
        {
            Logger.Error($"Can't connect to {url}");
            Logger.Error(e);

            return false;
        }
    }

    #endregion
}