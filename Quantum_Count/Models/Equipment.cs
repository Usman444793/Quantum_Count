using System.ComponentModel.DataAnnotations;
namespace Quantum_Count.Models;
public class Equipment
{
    public int Id { get; set; }
    [Required]
    [MaxLength(50)]
    public string EquipmentCode { get; set; } = string.Empty;
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(100)]
    public string? SerialNumber { get; set; } = string.Empty;
    [MaxLength(100)]
    public string? Brand { get; set; } = string.Empty;
    [MaxLength(100)]
    public string? Model { get; set; } = string.Empty;
    public DateTime? PurchaseDate { get; set; }
    public decimal PurchasePrice { get; set; } = 0;
    [MaxLength(50)]
    public string Status { get; set; } = "Available";
    [MaxLength(500)]
    public string? Description { get; set; } = string.Empty;
    public bool isActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; } 
    public int CategoryId { get; set; }
    public InventoryCategory? Category { get; set; }
}