using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;

namespace Quantum_Count.Services;

public class TokenStorage : ITokenStorage
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDataProtector _protector;
    private readonly IJSRuntime _jsRuntime;

    public TokenStorage(IDataProtectionProvider dataProtectionProvider, IHttpContextAccessor httpContextAccessor, IJSRuntime jsRuntime)
    {
        _protector = dataProtectionProvider.CreateProtector("Quantum_Count.TokenStorage");
        _httpContextAccessor = httpContextAccessor;
        _jsRuntime = jsRuntime;
    }

    public async Task SetItemAsync(string key, string value)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null)
        {
            try
            {
                var protectedValue = _protector.Protect(value);
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = httpContext.Request.IsHttps,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddDays(30)
                };
                httpContext.Response.Cookies.Append(key, protectedValue, cookieOptions);
                return;
            }
            catch
            {
                // fallback to JS storage below
            }
        }

        // fallback to client-side localStorage
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, value);
    }

    public async Task<string?> GetItemAsync(string key)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null)
        {
            try
            {
                if (httpContext.Request.Cookies.TryGetValue(key, out var protectedValue))
                {
                    try
                    {
                        var unprotected = _protector.Unprotect(protectedValue);
                        return unprotected;
                    }
                    catch
                    {
                        return null;
                    }
                }
            }
            catch
            {
                // fallback to JS storage below
            }
        }

        try
        {
            return await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", key);
        }
        catch
        {
            return null;
        }
    }

    public async Task RemoveItemAsync(string key)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null)
        {
            try
            {
                httpContext.Response.Cookies.Delete(key);
                return;
            }
            catch
            {
                // fallback to JS
            }
        }

        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
    }
}
