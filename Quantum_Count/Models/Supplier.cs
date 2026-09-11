using System.ComponentModel.DataAnnotations;
namespace Quantum_Count.Models;
public class Supplier
{
    public int Id { get; set; }
    [Required]
    [MaxLength(50)]
    public string SupplierCode { get; set; } = string.Empty;
    [Required]
    [MaxLength(150)]
    public string CompanyName { get; set; } = string.Empty;
    [MaxLength(100)]
    public string? ContactPerson { get; set; } = string.Empty;
    [MaxLength(20)]
    public string? Phone { get; set; } = string.Empty;
    [MaxLength(150)]
    [EmailAddress]
    public string? Email { get; set; } = string.Empty;
    [MaxLength(500)]
    public string? Address { get; set; } = string.Empty;
    [MaxLength(100)]
    public string? TaxNumber { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
    public ICollection<PurchaseOrder> PurchaseOrders { get; set; }  = new List<PurchaseOrder>();
}