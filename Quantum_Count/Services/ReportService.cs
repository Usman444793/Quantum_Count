using Microsoft.EntityFrameworkCore;
using Quantum_Count.Data;
using Quantum_Count.DTO.Reports;
namespace Quantum_Count.Services;
public class ReportsService
{
    private readonly ApplicationDbContext _context;
    public ReportsService(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<InventorySummaryDto> GetInventorySummaryAsync()
    {
        var materials = await _context.Materials.Include(m => m.Category)
            .Where(m => m.isActive).AsNoTracking().ToListAsync();
        var result = new InventorySummaryDto
        {
            TotalMaterials = materials.Count,
            TotalQuantity = materials.Sum(m => m.Quantity),
            TotalInventoryValue = materials.Sum( m => m.Quantity * m.UnitPrice),
            LowStockItems = materials.Count(m => m.Quantity > 0 && m.Quantity <= m.MinimumStockLevel),
            OutOfStockItems = materials.Count(m => m.Quantity <= 0)
        };
        result.Categories = materials.GroupBy(m => new
            {
                Id = m.CategoryId,
                Name = m.Category != null ? m.Category.Name : "Uncategorized"
            })
            .Select(g => new CategoryInventoryDto
            {
                CategoryId = g.Key.Id,
                CategoryName = g.Key.Name,
                MaterialCount = g.Count(),
                TotalQuantity = g.Sum(m => m.Quantity),
                TotalValue = g.Sum( m => m.Quantity * m.UnitPrice)
            }).OrderByDescending(c => c.TotalValue).ToList();
        return result;
    }
    public async Task<List<LowStockItemDto>> GetLowStockReportAsync()
    {
        var materials = await _context.Materials
            .Include(m => m.Category).Where(m => m.isActive && m.Quantity <= m.MinimumStockLevel)
            .AsNoTracking().OrderBy(m => m.Quantity).ToListAsync();
        return materials.Select(m => new LowStockItemDto
            {
                MaterialId = m.Id,
                MaterialCode = m.MaterialCode,
                MaterialName = m.Name,
                CategoryName = m.Category != null ? m.Category.Name : "Uncategorized",
                CurrentQuantity = m.Quantity,
                MinimumStockLevel = m.MinimumStockLevel,
                MaximumStockLevel = m.MaximumStockLevel,
                Unit = m.Unit,
                UnitPrice = m.UnitPrice,
                StockValue = m.Quantity * m.UnitPrice,
                Status = m.Quantity <= 0 ? "Out of Stock" : "Low Stock"
            }).ToList();
    }
    public async Task<List<StockMovementDto>> GetStockMovementReportAsync( DateTime? fromDate = null, DateTime? toDate = null)
    {
        var query = _context.InventoryTransactions.AsNoTracking().AsQueryable();
        if (fromDate.HasValue)
        {
            var from = fromDate.Value.Date;
            query = query.Where(t => t.CreatedAt >= from);
        }
        if (toDate.HasValue)
        {
            var to = toDate.Value.Date.AddDays(1);
            query = query.Where(t => t.CreatedAt < to);
        }
        var transactions = await query.ToListAsync();
        return transactions.Where(t => t.CreatedAt.HasValue).GroupBy(t => t.CreatedAt.Value.Date)
        .Select(g => new StockMovementDto
            {
                Date = g.Key,
                StockIn = g
                    .Where(t => t.TransactionType == "Stock In")
                    .Sum(t => t.Quantity),
                StockOut = g
                    .Where(t => t.TransactionType == "Stock Out")
                    .Sum(t => t.Quantity),
                Returns = g
                    .Where(t => t.TransactionType == "Return")
                    .Sum(t => t.Quantity),
                Damaged = g
                    .Where(t => t.TransactionType == "Damaged")
                    .Sum(t => t.Quantity),
                Adjustments = g
                    .Where(t => t.TransactionType == "Adjustment")
                    .Sum(t => t.Quantity),
                NetMovement =
                    g.Sum(t =>
                    t.TransactionType == "Stock Out" || t.TransactionType == "Damaged" ? -t.Quantity : t.Quantity)
            }).OrderBy(x => x.Date).ToList();
    }
    public async Task<List<StockTransactionReportDto>>
        GetStockTransactionReportAsync(DateTime? fromDate = null,DateTime? toDate = null)
    {
        var query = _context.InventoryTransactions.Include(t => t.Material).Where(t => t.Material != null)
            .AsNoTracking().AsQueryable();
        if (fromDate.HasValue)
        {
            var from = fromDate.Value.Date;
            query = query.Where(t => t.CreatedAt >= from);
        }
        if (toDate.HasValue)
        {
            var to = toDate.Value.Date.AddDays(1);
            query = query.Where(t =>t.CreatedAt < to);
        }
        var transactions = await query.OrderByDescending(t => t.CreatedAt).ToListAsync();
        return transactions.Select(t => new StockTransactionReportDto
            {
                TransactionId = t.Id,
                Date = t.CreatedAt,
                MaterialId = t.MaterialId,
                MaterialCode = t.Material!.MaterialCode,
                MaterialName = t.Material.Name,
                TransactionType = t.TransactionType,
                Quantity = t.Quantity,
                Reference = t.Reference,
                CreatedBy = t.CreatedBy
            }).ToList();
    }
    public async Task<ProcurementSummaryDto>
        GetProcurementSummaryAsync(DateTime? fromDate = null, DateTime? toDate = null)
    {
        var query = _context.PurchaseOrders.Include(p => p.Supplier).Where(p => p.IsActive).AsNoTracking().AsQueryable();
        if (fromDate.HasValue)
        {
            var from = fromDate.Value.Date;
            query = query.Where(p => p.OrderDate >= from);
        }
        if (toDate.HasValue)
        {
            var to = toDate.Value.Date.AddDays(1);
            query = query.Where(p =>p.OrderDate < to);
        }
        var orders = await query.ToListAsync();
        var result = new ProcurementSummaryDto
        {
            TotalPurchaseOrders = orders.Count,
            TotalPurchaseValue = orders.Sum(p => p.TotalAmount),
            DraftOrders = orders.Count(p => p.Status == "Draft"),
            OrderedOrders = orders.Count(p => p.Status == "Ordered"),
            PartiallyReceivedOrders = orders.Count(p => p.Status == "Partially Received"),
            ReceivedOrders = orders.Count(p => p.Status == "Received"),
            CancelledOrders = orders.Count(p => p.Status == "Cancelled")
        };
        result.Suppliers = orders.Where(p => p.Supplier != null).GroupBy(p => new
            {
                Id = p.SupplierId,
                Name = p.Supplier!.CompanyName
            })
            .Select(g => new SupplierPurchaseDto
            {
                SupplierId = g.Key.Id,
                SupplierName = g.Key.Name,
                OrderCount = g.Count(),
                TotalPurchaseValue = g.Sum(p => p.TotalAmount)
            })
            .OrderByDescending(x => x.TotalPurchaseValue).ToList();
        return result;
    }
    public async Task<EquipmentSummaryDto>
        GetEquipmentSummaryAsync()
    {
        var equipment = await _context.Equipment.Where(e => e.isActive).AsNoTracking().ToListAsync();
        return new EquipmentSummaryDto
        {
            TotalEquipment = equipment.Count,
            Available = equipment.Count(e => e.Status == "Available"),
            InUse = equipment.Count(e => e.Status == "In Use"),
            Maintenance = equipment.Count(e => e.Status == "Maintenance"),
            Damaged = equipment.Count( e => e.Status == "Damaged"),
            Retired = equipment.Count(e => e.Status == "Retired"),
            TotalEquipmentValue = equipment.Sum(e => e.PurchasePrice)
        };
    }
    public async Task<ProjectSummaryDto> GetProjectSummaryAsync()
    {
        var projects = await _context.Projects.AsNoTracking().ToListAsync();
        return new ProjectSummaryDto
        {
            TotalProjects = projects.Count,
            ActiveProjects = projects.Count(p => p.isActive),
            InactiveProjects = projects.Count(p => !p.isActive)
        };
    }
}