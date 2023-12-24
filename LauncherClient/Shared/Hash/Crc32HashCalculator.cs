using System;
using System.IO;
using System.IO.Hashing;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Shared.Hash;

public class Crc32HashCalculator : IHashCalculator
{
    #region public methods

    public void CalculateAndSaveHash(string rootDirectory, string savePath)
    {
        ProjectHashData hashData = new();

        if (!Directory.Exists(rootDirectory))
            throw new FileNotFoundException($"Directory {rootDirectory} doesn't exists");

        var files = Directory.GetFiles(rootDirectory, "*.*", SearchOption.AllDirectories);
        if (files.Length == 0)
            return;

        byte[] fileBytes;

        for (int i = 0; i < files.Length; i++)
        {
            var absoluteFilePath = files[i];

            if(!File.Exists(absoluteFilePath))
                continue;

            fileBytes = File.ReadAllBytes(absoluteFilePath);

            hashData.Hash.Add(Path.GetRelativePath(rootDirectory, absoluteFilePath), Crc32.HashToUInt32(fileBytes));
        }

        try
        {
            var serializedHash = JsonConvert.SerializeObject(hashData);

            var saveDirectory = Path.GetDirectoryName(savePath);
            if (!string.IsNullOrEmpty(saveDirectory) && !Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }

            File.WriteAllText(savePath, serializedHash);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    #endregion

    #region IHashCalculator

    public async Task<ProjectHashData> CalculateHashesAsync(string rootDirectory, CalculateHashStatus hashStatus)
    {
        ProjectHashData hashData = new();

        if (!Directory.Exists(rootDirectory))
            return hashData;

        var files = Directory.GetFiles(rootDirectory, "*.*", SearchOption.AllDirectories);
        if (files.Length == 0)
            return hashData;

        byte[] fileBytes;
        double totalFileCount = files.Length - 1;

        for (int i = 0; i < files.Length; i++)
        {
            var absoluteFilePath = files[i];

            if(!File.Exists(absoluteFilePath))
                continue;

            fileBytes = await File.ReadAllBytesAsync(absoluteFilePath);

            hashData.Hash.Add(Path.GetRelativePath(rootDirectory, absoluteFilePath), Crc32.HashToUInt32(fileBytes));

            hashStatus.Set(i / totalFileCount, absoluteFilePath);
        }

        return hashData;
    }

    #endregion
}