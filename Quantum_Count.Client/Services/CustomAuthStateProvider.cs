using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
namespace Quantum_Count.Client.Services;
public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly LocalStorageService _localStorage;
    private static readonly ClaimsPrincipal Anonymous =new(new ClaimsIdentity());
    public CustomAuthStateProvider(LocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorage.GetItemAsync("quantum_auth_token");
        if (string.IsNullOrWhiteSpace(token))
        {
            return new AuthenticationState(Anonymous);
        }
        var user = CreateClaimsPrincipal(token);
        return new AuthenticationState(user);
    }
    public async Task MarkUserAsAuthenticated(string token)
    {
        await _localStorage.SetItemAsync("quantum_auth_token",token);
        var user = CreateClaimsPrincipal(token);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }
    public async Task MarkUserAsLoggedOut()
    {
        await _localStorage.RemoveItemAsync("quantum_auth_token");
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(Anonymous)));
    }
    private ClaimsPrincipal CreateClaimsPrincipal(string token)
    {
        try
        {
            var parts = token.Split('.');
            if (parts.Length != 3)
            {
                return Anonymous;
            }
            var payload = parts[1];
            payload = payload.Replace('-', '+').Replace('_', '/');
            switch (payload.Length % 4)
            {
                case 2:
                    payload += "==";
                    break;

                case 3:
                    payload += "=";
                    break;
            }
            var jsonBytes = Convert.FromBase64String(payload);
            var payloadJson = Encoding.UTF8.GetString(jsonBytes);
            using var document =JsonDocument.Parse(payloadJson);
            var root = document.RootElement;
            var claims = new List<Claim>();
            foreach (var property in root.EnumerateObject())
            {
                var value = property.Value.ToString();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    claims.Add(new Claim(property.Name, value));
                }
            }
            var identity = new ClaimsIdentity(claims,authenticationType: "jwt");

            return new ClaimsPrincipal(identity);
        }
        catch
        {
            return Anonymous;
        }
    }
}