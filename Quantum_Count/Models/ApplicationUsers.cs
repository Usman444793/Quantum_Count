using Microsoft.AspNetCore.Identity;
namespace Quantum_Count.Models
{
    public class ApplicationUsers:IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
