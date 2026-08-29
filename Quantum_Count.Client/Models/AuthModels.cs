using System.ComponentModel.DataAnnotations;

namespace Quantum_Count.Client.Models;
public class LoginRequest
{
    [Required(ErrorMessage = "Email is Required")]
    [EmailAddress(ErrorMessage = "Enter a valid email Address")]
    public string Email { get; set; } = string.Empty;
    [Required(ErrorMessage = "Password is requires")]
    public string Password { get; set; } = string.Empty;
}
public class RegisterRequest
{
    [Required(ErrorMessage = "Your Name is required")]
    public string FullName { get; set; } = string.Empty;
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Enter a valid Email Address")]
    public string Email { get; set; } = string.Empty;
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;
}
public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}