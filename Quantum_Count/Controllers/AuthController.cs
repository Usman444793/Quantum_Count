using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Quantum_Count.DTO;
using Quantum_Count.DTOs;
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
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            return BadRequest(new
            {
                message = "Full Name is required."
            });
        }
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequest(new
            {
                message = "Email is required."
            });
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new
            {
                message = "Password is required."
            });
        }
        var existingUser =await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return BadRequest(new
            {
                message = "User already exists."
            });
        }
        var user = new ApplicationUsers
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            CreatedAt = DateTime.UtcNow
        };
        var result =
            await _userManager.CreateAsync(user,request.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return BadRequest(new
            {
                message = "Failed to create user.",
                errors
            });
        }
        var (token, expiresAt) =_jwtService.GenerateToken(user);
        return Ok(new AuthResponse
        {
            Token = token,
            ExpiresAt = expiresAt,
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email!
        });
    }
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new
            {
                message = "Email and Password are required."
            });
        }
        var user =await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Unauthorized(new
            {
                message = "Invalid Email or Password."
            });
        }
        var passwordValid =await _userManager.CheckPasswordAsync(user,request.Password);
        if (!passwordValid)
        {
            return Unauthorized(new
            {
                message = "Invalid Email or Password."
            });
        }
        var (token, expiresAt) =_jwtService.GenerateToken(user);
        return Ok(new AuthResponse
        {
            Token = token,
            ExpiresAt = expiresAt,
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email!
        });
    }
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userId =
            User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new
            {
                message = "User identity could not be determined."
            });
        }
        var user =
            await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound(new
            {
                message = "User not found."
            });
        }
        return Ok(new
        {
            userId = user.Id,
            fullName = user.FullName,
            email = user.Email
        });
    }
}