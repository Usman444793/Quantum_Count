namespace Quantum_Count.DTO.Reports;

public class InventorySummaryDto
{
    public int TotalMaterials { get; set; }
    public decimal TotalQuantity { get; set; }
    public decimal TotalInventoryValue { get; set; }
    public int LowStockItems { get; set; }
    public int OutOfStockItems { get; set; }

    public List<CategoryInventoryDto> Categories { get; set; } = new();
}

public class CategoryInventoryDto
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;

    public int MaterialCount { get; set; }
    public decimal TotalQuantity { get; set; }
    public decimal TotalValue { get; set; }
}

public class LowStockItemDto
{
    public int MaterialId { get; set; }
    public string MaterialCode { get; set; } = string.Empty;
    public string MaterialName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;

    public decimal CurrentQuantity { get; set; }
    public decimal MinimumStockLevel { get; set; }
    public decimal MaximumStockLevel { get; set; }

    public string Unit { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public decimal StockValue { get; set; }

    public string Status { get; set; } = string.Empty;
}

public class StockMovementDto
{
    public DateTime Date { get; set; }

    public decimal StockIn { get; set; }
    public decimal StockOut { get; set; }
    public decimal Returns { get; set; }
    public decimal Damaged { get; set; }
    public decimal Adjustments { get; set; }

    public decimal NetMovement { get; set; }
}

public class StockTransactionReportDto
{
    public int TransactionId { get; set; }

    public DateTime? Date { get; set; }

    public int MaterialId { get; set; }
    public string MaterialCode { get; set; } = string.Empty;
    public string MaterialName { get; set; } = string.Empty;

    public string TransactionType { get; set; } = string.Empty;
    public decimal Quantity { get; set; }

    public string? Reference { get; set; }
    public string? CreatedBy { get; set; }
}

public class ProcurementSummaryDto
{
    public int TotalPurchaseOrders { get; set; }
    public decimal TotalPurchaseValue { get; set; }

    public int DraftOrders { get; set; }
    public int OrderedOrders { get; set; }
    public int PartiallyReceivedOrders { get; set; }
    public int ReceivedOrders { get; set; }
    public int CancelledOrders { get; set; }

    public List<SupplierPurchaseDto> Suppliers { get; set; } = new();
}

public class SupplierPurchaseDto
{
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;

    public int OrderCount { get; set; }
    public decimal TotalPurchaseValue { get; set; }
}

public class EquipmentSummaryDto
{
    public int TotalEquipment { get; set; }

    public int Available { get; set; }
    public int InUse { get; set; }
    public int Maintenance { get; set; }
    public int Damaged { get; set; }
    public int Retired { get; set; }

    public decimal TotalEquipmentValue { get; set; }
}

public class ProjectSummaryDto
{
    public int TotalProjects { get; set; }
    public int ActiveProjects { get; set; }
    public int InactiveProjects { get; set; }
}