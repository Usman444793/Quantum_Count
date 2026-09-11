using System.ComponentModel.DataAnnotations;
namespace Quantum_Count.Models;
public class ProjectMaterial
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public Project? Project { get; set; }
    public int MaterialId { get; set; }
    public Material? Material { get; set; }
    [Range(0.01,Double.MaxValue)]
    public decimal QuantityIssued { get; set; }
    public decimal QuantityReturned { get; set; }
    public decimal QuantityUsed { get; set; }
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReturnedAt { get; set; }
    [MaxLength(500)]
    public string Notes { get; set; } = string.Empty;
    public bool isActive { get; set; }
}