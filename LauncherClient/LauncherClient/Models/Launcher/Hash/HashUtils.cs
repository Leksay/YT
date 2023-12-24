using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Shared.Hash;
using Splat;

namespace LauncherClient.Models.Launcher;

public static class HashUtils
{
    #region attributes

    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();    

    #endregion

    #region public methods

    public static bool NeedRecalculateLocalHashes(int cacheLifeTime)
    {
        string hashFilePath = Path.GetTempPath();
        if (!TryLoadHashFromFile(hashFilePath, out ProjectHashData? localHashData))
            return true;

        DateTime checkTime = localHashData?.CheckDate ?? DateTime.Now;

        return (DateTime.Now - checkTime).Days >= cacheLifeTime;
    }

    public static bool TryLoadHashFromFile(string hashFilePath, out ProjectHashData? localHashData)
    {
        localHashData = null;

        if (!File.Exists(hashFilePath))
            return false;

        string serializedHash = string.Empty;

        try
        {
            serializedHash = File.ReadAllText(hashFilePath);
            localHashData = JsonConvert.DeserializeObject<ProjectHashData>(serializedHash);
        }
        catch (Exception e)
        {
            Logger.Error(e);
            return false;
        }

        return localHashData != null;
    }

    #endregion

    public static async Task<ProjectHashData> CalculateLocalHashes(string rootDirectory, Action<double, string>? onCalculateProgressChanged = null)
    {
        IHashCalculator? hashCalculator = Locator.Current.GetService<IHashCalculator>();
        if (hashCalculator == null)
        {
            Logger.Error("Can't resolve hash caulcator");
            throw new NullReferenceException("Hash calculator is null");
        }

        using var hashStatus = new CalculateHashStatus(onCalculateProgressChanged);

        if(!Directory.Exists(rootDirectory))
            FilesUtils.CreateDirectoryIfNotExists(rootDirectory);

        return await hashCalculator.CalculateHashesAsync(rootDirectory, hashStatus);
    }
}