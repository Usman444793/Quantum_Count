using System.Threading.Tasks;
namespace Quantum_Count.Services;
public interface ITokenStorage
{
    Task SetItemAsync(string key, string value);
    Task<string?> GetItemAsync(string key);
    Task RemoveItemAsync(string key);
}