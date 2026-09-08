using System.ComponentModel.DataAnnotations;
namespace Quantum_Count.DTO;
public class LoginRequest
{
    [Required(ErrorMessage = "Email is Required")]
    [EmailAddress(ErrorMessage = "Enter a valid email Address")]
    public string Email { get; set; } = string.Empty;
    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; internal set; }
}