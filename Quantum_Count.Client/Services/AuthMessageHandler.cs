using System.Net.Http.Headers;
namespace Quantum_Count.Client.Services;
public class AuthMessageHandler : DelegatingHandler
{
    private readonly LocalStorageService _localStorage;
    public AuthMessageHandler(LocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }
    protected override async Task<HttpResponseMessage>SendAsync(HttpRequestMessage request,CancellationToken cancellationToken)
    {
        var token = await _localStorage.GetItemAsync("quantum_auth_token");
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer",token);
        }
        return await base.SendAsync(request,cancellationToken);
    }
}