using System;
using System.IO;
using Newtonsoft.Json;
using NLog;
using Path = System.IO.Path;

namespace LauncherClient.Models.Launcher;

[Serializable]
public class AppConfig
{
    #region constants

    public const string DefaultGameFolderName = "Game";

    public const string DefaultExeFileName = "GameFromLaunch.exe";

    public const string DefaultHashFileName = "GameHash.json";

    public const string DefaultBaseUri = "http://localhost:128/";

    private const string DefaultDownloadFolderName = DefaultGameFolderName;

    private const string ConfigLocalPath = "Resources/AppConfig.json";

    private const string DefaultHashApiCall = "https://65457c62fe036a2fa95458fb.mockapi.io/remote_hash";

    private const int DefaultLocalHashLifetime = 3;
    

    #endregion

    #region properties

    private static string DefaultDownloadFolder => Path.Combine(Path.GetTempPath(), DefaultDownloadFolderName);
    private static string BaseDirectory => AppContext.BaseDirectory;

    public string BaseUri { get; set; }
    
    public string GamePath { get; set; }
    
    public string GameExePath { get; set; }
    
    public int LocalHashLifetime { get; set; }
    
    public string TempDownloadFolder { get; set; }

    public string LocalHashPath { get; set; }

    public string HashApiCall { get; set; }

    #endregion

    #region factory method

    public static AppConfig GetConfig()
    {
        var configPath = Path.Combine(BaseDirectory, ConfigLocalPath);

        if (!File.Exists(configPath))
            return new AppConfig().SaveFile(configPath);

        try
        {
            var config = JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText(configPath)) ?? new AppConfig();
            //todo: add config validation

            return config.SaveFile(configPath);
        }
        catch (Exception e)
        {
            LogManager.GetCurrentClassLogger().Error(e);
            return new AppConfig().SaveFile(configPath);
        }
    }
    
    #endregion

    #region constructors

    /// <summary>
    /// Create config with default values.
    /// </summary>
    private AppConfig()
    {
        BaseUri = DefaultBaseUri;
        GamePath = Path.Combine(BaseDirectory, DefaultGameFolderName);
        GameExePath = Path.Combine(GamePath, DefaultExeFileName);
        TempDownloadFolder = DefaultDownloadFolder;
        LocalHashLifetime = DefaultLocalHashLifetime;
        LocalHashPath = Path.Combine(BaseDirectory, DefaultHashFileName);
        HashApiCall = DefaultHashApiCall;
    }

    #endregion

    #region service methods

    private AppConfig SaveFile(string configPath)
    {
        FilesUtils.SaveTextFile(configPath, JsonConvert.SerializeObject(this));

        return this;
    }

    #endregion
}