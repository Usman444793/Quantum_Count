using System.ComponentModel.DataAnnotations;
namespace Quantum_Count.Models;

public class PurchaseOrderItem
{
    public int Id { get; set; }
    public int PurchaseOrderId { get; set; }
    public PurchaseOrder? PurchaseOrder { get; set; }
    public int MaterialId { get; set; }
    public Material? Material { get; set; }
    [Range(0.01, double.MaxValue)]
    public decimal QuantityOrdered { get; set; }
    public decimal QuantityReceived { get; set; }
    [Range(0, double.MaxValue)]
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    [MaxLength(500)]
    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}