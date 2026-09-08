using System.ComponentModel.DataAnnotations;
namespace Quantum_Count.Models;
public class Material
{
    public int Id { get; set; }
    [Required]
    [MaxLength(50)]
    public string MaterialCode { get; set; } = string.Empty;
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(500)]
    public string? Description { get; set; } = string.Empty;
    [Required]
    [MaxLength(50)]
    public string Unit { get; set; } = string.Empty;
    [Range(0, double.MaxValue, ErrorMessage = "Quantity cannot be negative")]
    public decimal Quantity { get; set; } = 0;
    [Range(0, double.MaxValue, ErrorMessage = "Minimum stock level cannot be negative")]
    public decimal MinimumStockLevel { get; set; } = 0;
    [Range(0, double.MaxValue, ErrorMessage = "Maximum stock level cannot be negative")]
    public decimal MaximumStockLevel { get; set; } = 0;
    [Range(0, double.MaxValue, ErrorMessage = "Unit price cannot be negative")]
    public decimal UnitPrice { get; set; } = 0;
    public bool isActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Please select a category.")]
    public int CategoryId { get; set; }
    public InventoryCategory? Category { get; set; }
}