
using Quantum_Count.DTO;
using System.Net.Http.Json;
using Blazored.LocalStorage;

namespace Quantum_Count.Services;

public class AuthService
{
    private readonly HttpClient _http;
    private readonly ITokenStorage _tokenStorage;
    private const string TokenKey = "authToken";
    private const string UserNameKey = "quantumcount_name";
    private const string UserEmailKey = "quantumcount_email";
    private const string UserIdKey = "quantumcount_userid";

    public AuthService(HttpClient http, ITokenStorage tokenStorage)
    {
        _http = http;
        _tokenStorage = tokenStorage;
    }

    public async Task<AuthResponse?> LoginAsync(string email, string password)
    {
        try
        {
            var request = new LoginRequest
            {
                Email = email,
                Password = password
            };

            var response = await _http.PostAsJsonAsync("api/auth/login", request);

            if (!response.IsSuccessStatusCode)
                return null;

            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
            if (result is null)
                return null;

            await SaveSessionAsync(result);
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Login error: {ex.Message}");
            return null;
        }
    }

    public async Task<AuthResponse?> RegisterAsync(string fullName, string email, string password)
    {
        try
        {
            var request = new RegisterRequest
            {
                FullName = fullName,
                Email = email,
                Password = password
            };

            var response = await _http.PostAsJsonAsync("api/auth/register", request);

            if (!response.IsSuccessStatusCode)
                return null;

            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
            if (result is null)
                return null;

            await SaveSessionAsync(result);
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Register error: {ex.Message}");
            return null;
        }
    }

    private async Task SaveSessionAsync(AuthResponse result)
    {
        await _tokenStorage.SetItemAsync(TokenKey, result.Token);
        await _tokenStorage.SetItemAsync(UserIdKey, result.UserId);
        await _tokenStorage.SetItemAsync(UserNameKey, result.FullName);
        await _tokenStorage.SetItemAsync(UserEmailKey, result.Email);
    }

    public async Task<string?> GetTokenAsync()
    {
        return await _tokenStorage.GetItemAsync(TokenKey);
    }

    public async Task<string?> GetUserNameAsync()
    {
        return await _tokenStorage.GetItemAsync(UserNameKey);
    }

    public async Task<string?> GetUserEmailAsync()
    {
        return await _tokenStorage.GetItemAsync(UserEmailKey);
    }

    public async Task<string?> GetUserIdAsync()
    {
        return await _tokenStorage.GetItemAsync(UserIdKey);
    }

    public async Task<bool> IsLoggedInAsync()
    {
        var token = await GetTokenAsync();
        return !string.IsNullOrWhiteSpace(token);
    }
    public async Task<bool> LogoutAsync()
    {
        try
        {
            var response = await _http.PostAsync(
                "api/auth/logout",
                null);
            await _tokenStorage.RemoveItemAsync(TokenKey);
            await _tokenStorage.RemoveItemAsync(UserIdKey);
            await _tokenStorage.RemoveItemAsync(UserNameKey);
            await _tokenStorage.RemoveItemAsync(UserEmailKey);

            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
