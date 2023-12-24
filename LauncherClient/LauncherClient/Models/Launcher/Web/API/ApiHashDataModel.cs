using System;
using Newtonsoft.Json;
using Shared.Hash;

namespace LauncherClient.Models.Launcher;

[Serializable]
public class ApiHashDataModel
{
    #region properties

    [JsonProperty("remote_hash")]
    public ProjectHashData RemoteHash { get; set; }

    [field: NonSerialized] public static ApiHashDataModel Empty { get; } = new();

    #endregion
}