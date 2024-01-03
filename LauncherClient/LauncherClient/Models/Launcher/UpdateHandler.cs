using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LauncherClient.Models.Launcher.UI;
using LauncherClient.ViewModels;
using Shared.Hash;
using Splat;

namespace LauncherClient.Models.Launcher;

public class UpdateHandler
{
    #region attributes

    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    private readonly MainWindowViewModel _viewModel;
    private readonly IUIDispatcher _uiDispatcher;
    private readonly IApiHandler _apiHandler;
    private readonly AppConfig _appConfig;

    #endregion

    #region constructors

    public UpdateHandler()
    {
        _appConfig = Locator.Current.GetService<AppConfig>();
        _apiHandler = Locator.Current.GetService<IApiHandler>();
        _uiDispatcher = Locator.Current.GetService<IUIDispatcher>();
        _viewModel = Locator.Current.GetService<MainWindowViewModel>();

        if (_appConfig is null || _apiHandler is null || _uiDispatcher is null || _viewModel is null)
        {
            Logger.Fatal("Can't resolve service.");
            throw new NullReferenceException("Can't resolve service.");
        }
        
        Task.Factory.StartNew(HandleUpdate);
    }

    #endregion

    #region service methods

    private async Task HandleUpdate()
    {
        ProjectHashData? localHashData = await HandleLocalCachesAsync();
        if (localHashData == null)
        {
            Logger.Fatal("Can't load local hash");
            throw new NullReferenceException("Can't load local hash");
        }

        ApiHashDataModel remoteHash = await _apiHandler.LoadRemoteHash();
        HashDifference hashDifference = GetHashDifferences(localHashData, remoteHash);

        Logger.Info("Files changes count {0}", hashDifference.ChangesCount);

        if (hashDifference.ChangesCount == 0)
        {
            SetGameReady();
            return;
        }

        var downloadHandler = new DownloadHandler(_appConfig.BaseUri, ref hashDifference, _appConfig.TempDownloadFolder);

        List<string> missingFiles = await downloadHandler.LoadFilesAsync(UpdateUi);

        Logger.Info("Load all the files. Missing files count: {0}", missingFiles.Count);

        if (missingFiles.Count > 0)
        {
            var missingFilesString = missingFiles.SelectMany(missingFile => $"{missingFile},\n").ToString();
            Logger.Error("Can't load all game files. Missing files: {0}", missingFilesString);

            UpdateUi(0, "Can't load all game files");
            return;
        }

        if (!HandleFiles(ref hashDifference))
        {
            UpdateUi(0, "Can't complete files operation");
            Logger.Error("Can't complete files operation");

            return;
        }

        var newHash = remoteHash.RemoteHash;
        newHash.CheckDate = DateTime.Now;

        FilesUtils.SaveHash(remoteHash.RemoteHash, _appConfig.LocalHashPath);
        SetGameReady();
    }

    private bool HandleFiles(ref HashDifference hashDifference)
    {
        return FilesUtils.RemoveFiles(hashDifference.RemovedFiles, _appConfig.GamePath)
               && FilesUtils.MoveFiles(_appConfig.TempDownloadFolder, _appConfig.GamePath);
    }

    private HashDifference GetHashDifferences(ProjectHashData localHashData, ApiHashDataModel remoteHash)
    {
        if (remoteHash == ApiHashDataModel.Empty || remoteHash.RemoteHash == null)
        {
            Logger.Fatal("Remote hash is null");
            throw new NullReferenceException("Remote hash is null");
        }

        return localHashData.GetHashDifference(remoteHash.RemoteHash);
    }

    private async Task<ProjectHashData?> HandleLocalCachesAsync()
    {
        if (HashUtils.NeedRecalculateLocalHashes(_appConfig.LocalHashLifetime))
        {
            Logger.Info("Recalculating hashes");
            ProjectHashData localHash = await HashUtils.CalculateLocalHashes(_appConfig.GamePath, UpdateUi);
            FilesUtils.SaveHash(localHash, _appConfig.LocalHashPath);
        }

        if (!HashUtils.TryLoadHashFromFile(_appConfig.LocalHashPath, out ProjectHashData? localHashData)) 
            Logger.Error("Can't load local hashes");

        return localHashData;
    }

    private void SetGameReady()
    {
        Logger.Info("Game files are ready");

        _viewModel.FilesIsReady = true;
        UpdateUi(1, "Files are ready");
    }

    private void UpdateUi(double normalizedProgress, string message)
    {
        _uiDispatcher.ExecuteOnUiThread(() =>
        {
            _viewModel.Progress = normalizedProgress;
            _viewModel.Message = message;
        });
    }

    #endregion
}