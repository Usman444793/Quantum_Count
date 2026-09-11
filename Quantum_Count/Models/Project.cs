using System.ComponentModel.DataAnnotations;
namespace Quantum_Count.Models;
public class Project
{
    public int Id { get; set; }
    [Required]
    [MaxLength(50)]
    public string ProjectCode { get; set; } = string.Empty;
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [Required]
    [MaxLength(1000)]
    public string? Description { get; set; } = string.Empty;
    [MaxLength(150)]
    public string ClientOrDepartment { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Planning";
    public decimal Budget { get; set; }
    public bool isActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<ProjectMaterial> Materials { get; set; } = new List<ProjectMaterial>();
    public ICollection<ProjectEquipment> Equipment { get; set; } = new List<ProjectEquipment>();
    public ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
}