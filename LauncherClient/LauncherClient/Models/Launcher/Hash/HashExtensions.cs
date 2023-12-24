using System.Collections.Generic;
using System.Linq;
using Shared.Hash;

namespace LauncherClient.Models.Launcher;

public static class HashExtensions
{
    #region public methods

    public static HashDifference GetHashDifference(this ProjectHashData hash1, ProjectHashData hash2)
    {
        var keys1 = hash1.Hash.Keys;
        var keys2 = hash2.Hash.Keys;

        var addedFiles = new List<string>();
        var changedFiles = new List<string>();
        var removedFiles = new List<string>();

        addedFiles.AddRange(keys2.Except(keys1));
 
        var sameKeys = keys1.Intersect(keys2);
        var changedKeys = sameKeys.Where(key => hash1.Hash[key] != hash2.Hash[key]);

        changedFiles.AddRange(changedKeys);

        removedFiles.AddRange(keys1.Except(keys2));

        return new HashDifference(addedFiles, changedFiles, removedFiles);
    }

    #endregion
}