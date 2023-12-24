using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Hash;

[Serializable]
public class ProjectHashData
{
    #region properties

    public DateTime CheckDate { get; set; } = DateTime.Now;

    public Dictionary<string, uint> Hash { get; set; } = new Dictionary<string, uint>();

    #endregion

    #region public methods

    public override string ToString()
    {
        if (Hash.Count == 0)
            return "Hash is empty";

        StringBuilder sb = new StringBuilder();
        foreach (var hash in Hash) 
            sb.AppendLine($"{hash.Key} -> {hash.Value}");

        return sb.ToString();
    }

    #endregion

}