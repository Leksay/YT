using System.Threading.Tasks;

namespace Shared.Hash;
public interface IHashCalculator
{
    public Task<ProjectHashData> CalculateHashesAsync(string rootDirectory, CalculateHashStatus hashStatus);
}