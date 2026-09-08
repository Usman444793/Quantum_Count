using System.ComponentModel.DataAnnotations;
namespace Quantum_Count.Models;
public class InventoryTransactions
{
    public int Id { get; set; }
    public int MaterialId { get; set; }
    public Material? Material { get; set; }
    [Required]
    [MaxLength(30)]
    public string TransactionType { get; set; } = string.Empty;
    [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public decimal Quantity { get; set; }
    [MaxLength(100)]
    public string? Reference { get; set; }
    [MaxLength(100)]
    public string? Notes { get; set; }
    public DateTime? CreatedAt { get; set; }
    [MaxLength(100)]
    public string? CreatedBy { get; set; }
}