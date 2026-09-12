using Microsoft.EntityFrameworkCore;
using Quantum_Count.Data;
using Quantum_Count.DTO.Dashboard;
namespace Quantum_Count.Services;
public class DashboardService
{
    private readonly ApplicationDbContext _context;
    public DashboardService (ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<DashboardDto> GetDashboardAsync()
    {
        var materials = await _context.Materials.Where(m => m.isActive).AsNoTracking().ToListAsync();
        var equipment = await _context.Equipment.Where(e => e.isActive).AsNoTracking().ToListAsync();
        var projects = await _context.Projects.AsNoTracking().ToListAsync();
        var purchaseOrders = await _context.PurchaseOrders.AsNoTracking().ToListAsync();
        var transactions = await _context.InventoryTransactions.Include(t => t.Material).AsNoTracking()
            .OrderBy(t => t.CreatedAt).Take(8).ToListAsync();
        var result = new DashboardDto
        {
            TotalMaterials = materials.Count,
            LowStockItems = materials.Count(m => m.Quantity > 0 && m.Quantity <= m.MinimumStockLevel),
            OutOfStockItems = materials.Count(m => m.Quantity <=0),
            TotalInventoryValue = materials.Sum(m => m.Quantity * m.UnitPrice),
            TotalEquipment = equipment.Count,
            AvailableEquipment = equipment.Count(e => e.Status == "Available"),
            EquipmentInUse = equipment.Count(e => e.Status == "In Use"),
            TotalProjects = projects.Count,
            ActiveProjects = projects.Count(p => p.isActive),
            TotalPurchaseOrders = purchaseOrders.Count,
            PendingPurchaseOrders = purchaseOrders.Count(p => p.Status == "Draft" || p.Status == "Ordered" || p.Status == "Partially Received"),
            TotalPurchaseValue = purchaseOrders.Sum(p => p.TotalAmount)
        };
        result.RecentTransactions = transactions.Select(t => new DashboardTransactionDto
        {
            Date = t.CreatedAt ?? DateTime.UtcNow,
            MaterialName = t.Material?.Name ?? "Unknown",
            TransactionType = t.TransactionType,
            Quantity = t.Quantity
        }).ToList();
        result.ProjectProgress = new();
        result.Alerts = new();
        foreach(var material in materials.Where(m => m.Quantity <= m.MinimumStockLevel ).OrderBy(m => m.Quantity).Take(5))
        {
            result.Alerts.Add(new DashboardAlertDto
            {
                Type = material.Quantity <= 0 ? "danger" : "warning",
                message = material.Quantity <= 0 ? $"{material.Name} is out of stock" : $"{material.Name} is below minimum stock"
            });
        }
        foreach (var item in equipment.Where(e => e.Status == "Maintanence" || e.Status == "Damaged").Take(5))
        {
            result.Alerts.Add(new DashboardAlertDto
            {
                Type = "warning",
                message = $"{item.Name} requires attention"
            });
        }
        return result;
    }
}