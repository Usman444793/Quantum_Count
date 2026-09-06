using System.Net.Http.Headers;
namespace Quantum_Count.Services;
public class AuthMessageHandler : DelegatingHandler
{
    private readonly ITokenStorage _tokenStorage;
    private const string TokenKey = "authToken";
    public AuthMessageHandler(ITokenStorage tokenStorage)
    {
        _tokenStorage = tokenStorage;
    }
    protected override async Task<HttpResponseMessage>
        SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _tokenStorage.GetItemAsync(TokenKey);
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        return await base.SendAsync(request, cancellationToken);
    }
}
