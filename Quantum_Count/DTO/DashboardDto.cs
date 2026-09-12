namespace Quantum_Count.DTO.Dashboard;
public class DashboardDto
{
    public int TotalMaterials { get; set; }
    public int LowStockItems { get; set; }
    public int OutOfStockItems { get; set; }
    public int TotalEquipment { get; set; }
    public int AvailableEquipment { get; set; }
    public int EquipmentInUse { get; set; }
    public int TotalProjects { get; set; }
    public int ActiveProjects { get; set; }
    public int TotalPurchaseOrders { get; set; }
    public int PendingPurchaseOrders { get; set; }
    public decimal TotalInventoryValue { get; set; }
    public decimal TotalPurchaseValue { get; set; }
    public List<DashboardTransactionDto> RecentTransactions { get; set; } = new();
    public List<DashboardProjectDto> ProjectProgress { get; set; } = new();
    public List<DashboardAlertDto> Alerts { get; set; } = new();
}
public class DashboardTransactionDto
{
    public DateTime? Date { get; set; }
    public string MaterialName { get; set; } = string.Empty;
    public string TransactionType { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
}
public class DashboardProjectDto
{
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public decimal Progress { get; set; } 
}
public class DashboardAlertDto
{
    public string Type { get; set; } = string.Empty;
    public string message { get; set; } = string.Empty;
}