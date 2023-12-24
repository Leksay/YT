using System.Threading.Tasks;

namespace LauncherClient.Models.Launcher;

public interface IApiHandler
{
    public Task<ApiHashDataModel> LoadRemoteHash();
}