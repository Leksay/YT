using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace LauncherClient.Models.Launcher;

public class DownloadHandler
{
    #region properties

    public int BytesLoaded { get; private set; }
    public double? LoadFileProgress { get; private set; }

    #endregion

    #region attributes

    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    private readonly string _baseUri;
    private readonly HashDifference _hashDifference;
    private readonly string _tempDirectory;

    #endregion

    #region constructors

    public DownloadHandler(string baseUri, ref HashDifference hashDifference, string tempDirectory)
    {
        _baseUri = baseUri;
        _hashDifference = hashDifference;
        _tempDirectory = tempDirectory;
    }

    #endregion

    #region public methods

    public async Task<List<string>> LoadFilesAsync(Action<double, string>? updateUiAction = null)
    {
        if (!await WebUtils.CheckForConnection(_baseUri))
        {
            Logger.Error($"Can't connect to {_baseUri}");

            updateUiAction?.Invoke(0, "Can't connect to server");

            throw new Exception($"Can't connect to {_baseUri}");
        }

        int filesLoaded = 0;
        int totalFilesCount = _hashDifference.AddedFiles.Count + _hashDifference.ChangedFiles.Count;

        var filesToLoad = _hashDifference.AddedFiles.Concat(_hashDifference.ChangedFiles).ToArray();
        var filesCantBeLoaded = new List<string>();

        Logger.Info("Start load files. Need to load {0} files", filesToLoad);
        
        foreach (string localFilePath in filesToLoad)
        {
            string loadUri = $"{_baseUri}/{localFilePath}";
            string savePath = Path.Combine(_tempDirectory, localFilePath);

            FilesUtils.CreateDirectoryIfNotExists(savePath);

            if (!await LoadFile(loadUri, savePath))
            {
                filesCantBeLoaded.Add(localFilePath);
                continue;
            }

            filesLoaded++;
            double totalProgress = (double)filesLoaded / totalFilesCount;

            updateUiAction?.Invoke(totalProgress, loadUri);
        }

        if (filesCantBeLoaded.Count == 0 || filesCantBeLoaded.Count == totalFilesCount)
            return filesCantBeLoaded;

        // Try to load files second time
        for (int i = filesCantBeLoaded.Count - 1; i >= 0; i--)
        {
            string loadUri = $"{_baseUri}/{filesCantBeLoaded[i]}";
            string savePath = Path.Combine(_tempDirectory, filesCantBeLoaded[i]);

            if (await LoadFile(loadUri, savePath))
                filesCantBeLoaded.RemoveAt(i);
        }

        return filesCantBeLoaded;
    }

    #endregion

    #region service methods

    private async Task<bool> LoadFile(string uri, string savePath)
    {
        using var httpClient = new HttpClient();
        HttpResponseMessage? response = null;

        Logger.Info($"Start load file {uri} to {savePath}");

        try
        {
            response = await httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception e)
        {
            Logger.Error($"Can't load file {uri}. Status code: {response?.StatusCode}");
            Logger.Error(e);

            return false;
        }

        // file size in byte -> Mb
        var bytesToLoad = response.Content.Headers.ContentLength / 1024 / 1024;

        const int bufferSize = 8192;

        var buffer = new byte[bufferSize];
        int bytesRead;

        await using Stream contentStream = await response.Content.ReadAsStreamAsync();
        await using FileStream fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize);

        try
        {
            do
            {
                bytesRead = await contentStream.ReadAsync(buffer, 0, bufferSize);
                if (bytesRead == 0)
                    continue;

                await fileStream.WriteAsync(buffer, 0, bytesRead);

                BytesLoaded += bytesRead;
                if (bytesToLoad is > 0)
                    LoadFileProgress = (double)BytesLoaded / bytesToLoad;

            } while (bytesRead > 0);
        }
        catch (Exception e)
        {
            await contentStream.DisposeAsync();
            await fileStream.DisposeAsync();

            Logger.Error(e);
            throw;
        }

        response.Dispose();

        return true;
    }

    #endregion
}