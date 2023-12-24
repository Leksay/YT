using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Splat;

namespace LauncherClient.Models.Launcher;

public class  MockApiHandler : IApiHandler
{
    #region attributes

    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    private readonly string? _apiCallUri;

    #endregion

    #region constructors

    public MockApiHandler()
    {
        _apiCallUri = Locator.Current.GetService<AppConfig>()?.HashApiCall;

        if (string.IsNullOrEmpty(_apiCallUri))
        {
            Logger.Error("Config file provides empty API call link.");
            throw new NullReferenceException("API link is null or empty");
        }
    }

    #endregion

    #region IApiHandler

    public async Task<ApiHashDataModel> LoadRemoteHash()
    {
        using var httpClient = new HttpClient();

        Logger.Info("Load remote hash data. Uri: {0}",_apiCallUri);

        using HttpResponseMessage response = await httpClient.GetAsync(_apiCallUri);

        if (!response.IsSuccessStatusCode)
        {
            Logger.Error("Wrong api response. Status code: {0}", response.StatusCode);
            return ApiHashDataModel.Empty;
        }

        string httpContent = await response.Content.ReadAsStringAsync();
        try
        {
            List<ApiHashDataModel>? deserializedHashes = JsonConvert.DeserializeObject<List<ApiHashDataModel>>(httpContent);
            if(deserializedHashes == null || deserializedHashes.Count == 0)
                return ApiHashDataModel.Empty;

            return deserializedHashes.First();
        }
        catch (Exception e)
        {
            Logger.Error("Can't deserialize api hash. {0}", e);
            throw;
        }
    }

    #endregion
}