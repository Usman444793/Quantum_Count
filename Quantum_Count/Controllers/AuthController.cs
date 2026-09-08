using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Quantum_Count.DTO;
using Quantum_Count.Models;
using Quantum_Count.Services;
using System.Security.Claims;
namespace Quantum_Count.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUsers> _userManager;
    private readonly JwtService _jwtService;
    public AuthController(UserManager<ApplicationUsers> userManager,JwtService jwtService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
    }
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            return BadRequest(new { message = "Full name is required." });
        }
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequest(new { message = "Email is required." });
        }
        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { message = "Password is required." });
        }
        var email = request.Email.Trim().ToLowerInvariant();
        var fullName = request.FullName.Trim();
        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            return Conflict(new { message = "An account with this email already exists." });
        }
        var user = new ApplicationUsers
        {
            UserName = email,
            Email = email,
            FullName = fullName,
            CreatedAt = DateTime.UtcNow
        };
        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(error => error.Description).ToList();
            return BadRequest(new { message = "Unable to create account.", errors });
        }
        var (token, expiresAt) = _jwtService.GenerateToken(user);
        SetJwtCookie(token, expiresAt);
        var response = new AuthResponse
        {
            Token = token,
            ExpiresAt = expiresAt,
            UserId = user.Id,
            FullName = user.FullName ?? "",
            Email = user.Email ?? ""
        };
        return StatusCode(StatusCodes.Status201Created, response);
    }
    [AllowAnonymous]
    [HttpPost("register-form")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegisterForm([FromForm] string FullName, [FromForm] string Email, [FromForm] string Password)
    {
        if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            return Redirect("/register?error=invalid");
        }
        var email = Email.Trim().ToLowerInvariant();
        var fullName = FullName.Trim();
        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            return Redirect("/register?error=exists");
        }
        var user = new ApplicationUsers
        {
            UserName = email,
            Email = email,
            FullName = fullName,
            CreatedAt = DateTime.UtcNow
        };
        var result = await _userManager.CreateAsync(user, Password);
        if (!result.Succeeded)
        {
            return Redirect("/register?error=failed");
        }
        var (token, expiresAt) = _jwtService.GenerateToken(user);
        SetJwtCookie(token, expiresAt);
        return Redirect("/login");
    }
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { message = "Email and password are required." });
        }
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }
        var (token, expiresAt) = _jwtService.GenerateToken(user);
        SetJwtCookie(token, expiresAt);
        return Ok(new AuthResponse
        {
            Token = token,
            ExpiresAt = expiresAt,
            UserId = user.Id,
            FullName = user.FullName ?? "",
            Email = user.Email ?? ""
        });
    }
    [AllowAnonymous]
    [HttpPost("login-form")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LoginForm([FromForm] string Email, [FromForm] string Password)
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            return Redirect("/login?error=invalid");
        }
        var email = Email.Trim().ToLowerInvariant();
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return Redirect("/login?error=invalid");
        }
        var passwordValid = await _userManager.CheckPasswordAsync(user, Password);
        if (!passwordValid)
        {
            return Redirect("/login?error=invalid");
        }
        var (token, expiresAt) = _jwtService.GenerateToken(user);
        SetJwtCookie(token, expiresAt);
        return Redirect("/dashboard");
    }
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "User identity could not be determined." });
        }
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound(new { message = "User not found." });
        }
        return Ok(new
        {
            userId = user.Id,
            fullName = user.FullName,
            email = user.Email ?? ""
        });
    }
    [HttpPost("logout")]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        Response.Cookies.Delete(JwtService.CookieName);
        return Redirect("/login");
    }
    [AllowAnonymous]
    [HttpGet("logout")]
    public IActionResult LogoutGet()
    {
        Response.Cookies.Delete(JwtService.CookieName);
        return Redirect("/login");
    }
    private void SetJwtCookie(string token, DateTime expiresAt)
    {
        Response.Cookies.Append(JwtService.CookieName, token, new CookieOptions
        {
            HttpOnly = true,
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.Lax,
            Expires = expiresAt
        });
    }
}