using System.ComponentModel.DataAnnotations;
namespace Quantum_Count.Models;
public class InventoryCategory
{
    public int Id { get; set; }
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(500)]
    public string? Description { get; set; } = string.Empty;
    public bool isActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Material> Materials { get; set; } = new List<Material>();
    public ICollection<Equipment> Equipment { get; set; } = new List<Equipment>();
}