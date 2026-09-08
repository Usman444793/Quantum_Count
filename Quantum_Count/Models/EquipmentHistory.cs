using System.ComponentModel.DataAnnotations;
namespace Quantum_Count.Models;
public class EquipmentHistory
{
    public int Id { get; set; }
    public int EquipmentId { get; set; }
    public Equipment? Equipment { get; set; }
    [Required]
    [MaxLength(50)]
    public string ActivityType { get; set; } = string.Empty;
    [MaxLength(50)]
    public string? Reference { get; set; } = string.Empty;
    [MaxLength(1000)]
    public string? Notes { get; set; } = string.Empty;
    [MaxLength(50)]
    public string? PerformedBy { get; set; } = string.Empty;
    public string? TransactionType { get; set; } = string.Empty;
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    [MaxLength(100)]
    public string CreatedBy { get; set; } = string.Empty;
}