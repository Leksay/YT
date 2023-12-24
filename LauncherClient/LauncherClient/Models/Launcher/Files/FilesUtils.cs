using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Shared.Hash;

namespace LauncherClient.Models.Launcher;

public static class FilesUtils
{
    #region attributes

    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    #endregion

    #region public methods

    public static void SaveHash(ProjectHashData hashData, string hashFilePath)
    {
        Task.Run(() =>
        {
            string serializedHash = JsonConvert.SerializeObject(hashData);
            File.WriteAllText(hashFilePath, serializedHash);
        });
    }

    public static void SaveTextFile(string path, string text)
    {
        CreateDirectoryIfNotExists(path);
        File.WriteAllText(path, text);
    }

    public static void CreateDirectoryIfNotExists(string path)
    {
        var directory = File.Exists(path) && File.GetAttributes(path).HasFlag(FileAttributes.Directory) ? path : Path.GetDirectoryName(path);

        if(string.IsNullOrEmpty(directory))
            return;

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
    }

    public static bool RemoveFiles(IEnumerable<string> filesToRemove, string directory)
    {
        if (!filesToRemove.Any())
            return true;

        if (!Directory.Exists(directory))
        {
            Logger.Error($"Can't remove files. Directory {directory} is doesnt exists");
            return false;
        }

        try
        {
            foreach (var localFilePath in filesToRemove)
            {
                var path = Path.Combine(directory, localFilePath);
                if (!File.Exists(path))
                {
                    Logger.Info($"Can't delete non existing file {path}");
                    continue;
                }

                File.Delete(path);
            }
        }
        catch (Exception e)
        {
            Logger.Error(e);
            return false;
        }

        return true;
    }

    public static bool MoveFiles(string fromDirectory, string toDirectory, bool deleteFromDirectoryAfterMove = true)
    {
        if (!Directory.Exists(fromDirectory))
        {
            Logger.Info($"Can't move file from {fromDirectory} because it doesn't exists");
            return false;
        }

        string[] filesToCopy = Directory.GetFiles(fromDirectory, "*.*", SearchOption.AllDirectories);

        try
        {
            foreach (var filePath in filesToCopy)
            {
                string relativePath = Path.GetRelativePath(fromDirectory, filePath);
                string destinationPath = Path.Combine(toDirectory, relativePath);

                CreateDirectoryIfNotExists(destinationPath);

                File.Move(filePath, destinationPath, true);
            }

            if(deleteFromDirectoryAfterMove)
                Directory.Delete(fromDirectory, true);

            return true;
        }
        catch (Exception e)
        {
            Logger.Error(e);
            return false;
        }
    }

    #endregion
}