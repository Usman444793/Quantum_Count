using System.ComponentModel.DataAnnotations;
namespace Quantum_Count.Models;
public class PurchaseOrder
{
    public int Id { get; set; }
    [Required]
    [MaxLength(50)]
    public string PONumber { get; set; } = string.Empty;
    public int SupplierId { get; set; }
    public Supplier? Supplier { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public DateTime? ExpectedDeliveryDate { get; set; }
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Draft";

    [MaxLength(1000)]
    public string? Notes { get; set; }

    public decimal TotalAmount { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public ICollection<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>();
}