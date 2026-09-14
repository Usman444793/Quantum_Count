using System.ComponentModel.DataAnnotations;
namespace Quantum_Count.Models;
public class ApplicationSettings
{
    public int Id { get; set; }
    [Required]
    [MaxLength(150)]
    public string OrganizationName { get; set; } = "Quantum Count";
    [Required]
    [MaxLength(10)]
    public string Currency { get; set; } = "PKR";
    [Required]
    [MaxLength(100)]
    public string TimeZone { get; set; } = "Pakistan Standard Time";
    [Required]
    [MaxLength(30)]
    public string DateFormat { get; set; } = "dd/MM/yyyy";
    public bool LowStockAlerts { get; set; } = true;
    public bool AllowNegativeStock { get; set; } = false;
    [Required]
    [MaxLength(30)]
    public string DefaultUnit { get; set; } = "pcs";
    public bool PurchaseOrderNotifications { get; set; } = true;
    public bool EquipmentMaintenanceNotifications { get; set; } = true;
    [Required]
    [MaxLength(20)]
    public string Theme { get; set; } = "Dark";
    public bool CompactSidebar { get; set; } = false;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
