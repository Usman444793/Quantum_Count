using System.ComponentModel.DataAnnotations;
namespace Quantum_Count.Models;
public class ProjectEquipment
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public Project? Project { get; set; }
    public int EquipmentId { get; set; }
    public Equipment? Equipment { get; set; }
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReturnedAt { get; set; }
    [MaxLength(500)]
    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true;
}