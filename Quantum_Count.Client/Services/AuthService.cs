using System.Net.Http.Json;
using Quantum_Count.Client.Models;

namespace Quantum_Count.Client.Services;

public class AuthService
{
    private readonly HttpClient _http;

    public AuthService(HttpClient http)
    {
        _http = http;
    }

    public async Task<AuthResponse?> LoginAsync(
        string email,
        string password)
    {
        var request = new LoginRequest
        {
            Email = email,
            Password = password
        };

        var response = await _http.PostAsJsonAsync(
            "api/auth/login",
            request);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content
            .ReadFromJsonAsync<AuthResponse>();
    }

    public async Task<AuthResponse?> RegisterAsync(
        string fullName,
        string email,
        string password)
    {
        var request = new RegisterRequest
        {
            FullName = fullName,
            Email = email,
            Password = password
        };

        var response = await _http.PostAsJsonAsync(
            "api/auth/register",
            request);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content
            .ReadFromJsonAsync<AuthResponse>();
    }
}