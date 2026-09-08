using System.ComponentModel.DataAnnotations;
namespace Quantum_Count.DTO;
public class RegisterRequest
{
    [Required(ErrorMessage = "Your Name is required")]
    public string FullName { get; set; } = string.Empty;
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Enter a valid Email Address")]
    public string Email { get; set; } = string.Empty;
    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; set; } = string.Empty;
}
