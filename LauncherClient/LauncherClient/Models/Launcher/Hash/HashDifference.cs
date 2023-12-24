using System.Collections.Generic;

namespace LauncherClient.Models.Launcher;

public readonly struct HashDifference
{
    #region properties

    public List<string> AddedFiles { get; }
    public List<string> ChangedFiles { get; }
    public List<string> RemovedFiles { get; }

    public int ChangesCount { get; }

    #endregion

    #region constructors

    public HashDifference(List<string> addedFiles, List<string> changedFiles, List<string> removedFiles)
    {
        AddedFiles = addedFiles;
        ChangedFiles = changedFiles;
        RemovedFiles = removedFiles;

        ChangesCount = AddedFiles.Count + ChangedFiles.Count + RemovedFiles.Count;
    }

    #endregion
}