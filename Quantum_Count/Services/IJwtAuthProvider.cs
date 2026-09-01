using System.Threading.Tasks;

namespace Quantum_Count.Services;

public interface IJwtAuthProvider
{
    Task LoginAsync(string token);
    Task LogoutAsync();
}
