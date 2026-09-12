using Microsoft.AspNetCore.Mvc;
using Quantum_Count.Services;
using Microsoft.AspNetCore.Authorization;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Quantum_Count.DTO.Reports;
namespace Quantum_Count.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly ReportsService _reportsService;
    public ReportsController(ReportsService reportsService)
    {
        _reportsService = reportsService;
    }
    [HttpGet("inventory-summary")]
    public async Task<ActionResult<InventorySummaryDto>> GetInventorySummary()
    {
        var report = await _reportsService.GetInventorySummaryAsync();
        return Ok(report);
    }
    [HttpGet("low-stock")]
    public async Task<ActionResult<List<LowStockItemDto>>> GetLowStockReport()
    {
        var report = await _reportsService.GetLowStockReportAsync();
        return Ok(report);
    }
    [HttpGet("stock-movements")]
    public async Task<ActionResult<List<StockMovementDto>>> GetStockMovementReport([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
    {
        if (fromDate.HasValue && toDate.HasValue && fromDate > toDate)
            return BadRequest(new
            {
                message = "From Date cannot be more than To Date."
            });
        var report = await _reportsService.GetStockMovementReportAsync(fromDate,toDate);
        return Ok(report);
    }
    [HttpGet("stock-transactions")]
    public async Task <ActionResult<List<StockTransactionReportDto>>> GetStockTransactions([FromQuery] DateTime? fromDate, [FromQuery]DateTime? toDate)
    {
        if (fromDate.HasValue && toDate.HasValue && fromDate > toDate)
        {
            return BadRequest(new
            {
                message = "From Date cannot be more than To Date."
            });
        }
        var report = await _reportsService.GetStockTransactionReportAsync(fromDate,toDate);
        return Ok(report);
    }
    [HttpGet("procurement-summary")]
    public async Task <ActionResult<ProcurementSummaryDto>> GetProcurementReport([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
    {
        if (fromDate.HasValue && toDate.HasValue && fromDate > toDate)
        {
            return BadRequest(new
            {
                message = "From Date connot be more than To Date."
            });
        }
        var report = await _reportsService.GetProcurementSummaryAsync(fromDate,toDate);
        return Ok(report);
    }
    [HttpGet("equipment-summary")]
    public async Task <ActionResult<EquipmentSummaryDto>> GetEquipmentSummary()
    {
        var report = await _reportsService.GetEquipmentSummaryAsync();
        return Ok(report);
    }
    [HttpGet("project-summary")]
    public async Task <ActionResult<ProjectSummaryDto>> GetProjectSummary()
    {
        var report = await _reportsService.GetProjectSummaryAsync();
        return Ok(report);
    }
    [HttpGet("export/excel")]
    public async Task<IActionResult> ExportExcel([FromQuery] DateTime? fromDate = null,[FromQuery] DateTime? toDate = null)
    {
        var inventory = await _reportsService.GetInventorySummaryAsync();
        var lowStock = await _reportsService.GetLowStockReportAsync();
        var movements = await _reportsService.GetStockMovementReportAsync(fromDate, toDate);
        var transactions = await _reportsService.GetStockTransactionReportAsync(fromDate, toDate);
        var procurement = await _reportsService.GetProcurementSummaryAsync(fromDate, toDate);
        var equipment = await _reportsService.GetEquipmentSummaryAsync();
        var projects = await _reportsService.GetProjectSummaryAsync();
        using var workbook = new XLWorkbook(); 
        var inventorySheet = workbook.Worksheets.Add("Inventory");
        inventorySheet.Cell(1, 1).Value = "Quantum Count";
        inventorySheet.Cell(2, 1).Value = "Inventory Report";
        inventorySheet.Cell(4, 1).Value = "Metric";
        inventorySheet.Cell(4, 2).Value = "Value";
        inventorySheet.Cell(5, 1).Value = "Total Materials";
        inventorySheet.Cell(5, 2).Value = inventory.TotalMaterials;
        inventorySheet.Cell(6, 1).Value = "Total Quantity";
        inventorySheet.Cell(6, 2).Value = inventory.TotalQuantity;
        inventorySheet.Cell(7, 1).Value = "Total Inventory Value";
        inventorySheet.Cell(7, 2).Value = inventory.TotalInventoryValue;
        inventorySheet.Cell(8, 1).Value = "Low Stock Items";
        inventorySheet.Cell(8, 2).Value = inventory.LowStockItems;
        inventorySheet.Cell(9, 1).Value = "Out of Stock Items";
        inventorySheet.Cell(9, 2).Value = inventory.OutOfStockItems;
        // Category Report
        var categorySheet = workbook.Worksheets.Add("Categories");
        categorySheet.Cell(1, 1).Value = "Category";
        categorySheet.Cell(1, 2).Value = "Materials";
        categorySheet.Cell(1, 3).Value = "Quantity";
        categorySheet.Cell(1, 4).Value = "Total Value";
        var categoryRow = 2;
        foreach (var category in inventory.Categories)
        {
            categorySheet.Cell(categoryRow, 1).Value = category.CategoryName;
            categorySheet.Cell(categoryRow, 2).Value = category.MaterialCount;
            categorySheet.Cell(categoryRow, 3).Value = category.TotalQuantity;
            categorySheet.Cell(categoryRow, 4).Value = category.TotalValue;
            categoryRow++;
        }
        // Low Stock
        var lowStockSheet = workbook.Worksheets.Add("Low Stock");
        lowStockSheet.Cell(1, 1).Value = "Code";
        lowStockSheet.Cell(1, 2).Value = "Material";
        lowStockSheet.Cell(1, 3).Value = "Category";
        lowStockSheet.Cell(1, 4).Value = "Current";
        lowStockSheet.Cell(1, 5).Value = "Minimum";
        lowStockSheet.Cell(1, 6).Value = "Maximum";
        lowStockSheet.Cell(1, 7).Value = "Unit";
        lowStockSheet.Cell(1, 8).Value = "Unit Price";
        lowStockSheet.Cell(1, 9).Value = "Stock Value";
        lowStockSheet.Cell(1, 10).Value = "Status";
        var lowRow = 2;
        foreach (var item in lowStock)
        {
            lowStockSheet.Cell(lowRow, 1).Value = item.MaterialCode;
            lowStockSheet.Cell(lowRow, 2).Value = item.MaterialName;
            lowStockSheet.Cell(lowRow, 3).Value = item.CategoryName;
            lowStockSheet.Cell(lowRow, 4).Value = item.CurrentQuantity;
            lowStockSheet.Cell(lowRow, 5).Value = item.MinimumStockLevel;
            lowStockSheet.Cell(lowRow, 6).Value = item.MaximumStockLevel;
            lowStockSheet.Cell(lowRow, 7).Value = item.Unit;
            lowStockSheet.Cell(lowRow, 8).Value = item.UnitPrice;
            lowStockSheet.Cell(lowRow, 9).Value = item.StockValue;
            lowStockSheet.Cell(lowRow, 10).Value = item.Status;
            lowRow++;
        }
        // Stock Movement
        var movementSheet = workbook.Worksheets.Add("Stock Movement");
        movementSheet.Cell(1, 1).Value = "Date";
        movementSheet.Cell(1, 2).Value = "Stock In";
        movementSheet.Cell(1, 3).Value = "Stock Out";
        movementSheet.Cell(1, 4).Value = "Returns";
        movementSheet.Cell(1, 5).Value = "Damaged";
        movementSheet.Cell(1, 6).Value = "Adjustments";
        movementSheet.Cell(1, 7).Value = "Net Movement";
        var movementRow = 2;
        foreach (var movement in movements)
        {
            movementSheet.Cell(movementRow, 1).Value = movement.Date;
            movementSheet.Cell(movementRow, 2).Value = movement.StockIn;
            movementSheet.Cell(movementRow, 3).Value = movement.StockOut;
            movementSheet.Cell(movementRow, 4).Value = movement.Returns;
            movementSheet.Cell(movementRow, 5).Value = movement.Damaged;
            movementSheet.Cell(movementRow, 6).Value = movement.Adjustments;
            movementSheet.Cell(movementRow, 7).Value = movement.NetMovement;
            movementRow++;
        }
        // Transactions
        var transactionSheet = workbook.Worksheets.Add("Transactions");
        transactionSheet.Cell(1, 1).Value = "Date";
        transactionSheet.Cell(1, 2).Value = "Code";
        transactionSheet.Cell(1, 3).Value = "Material";
        transactionSheet.Cell(1, 4).Value = "Type";
        transactionSheet.Cell(1, 5).Value = "Quantity";
        transactionSheet.Cell(1, 6).Value = "Reference";
        transactionSheet.Cell(1, 7).Value = "Created By";
        var transactionRow = 2;
        foreach (var transaction in transactions)
        {
            transactionSheet.Cell(transactionRow, 1).Value =transaction.Date;
            transactionSheet.Cell(transactionRow, 2).Value =transaction.MaterialCode;
            transactionSheet.Cell(transactionRow, 3).Value = transaction.MaterialName;
            transactionSheet.Cell(transactionRow, 4).Value = transaction.TransactionType;
            transactionSheet.Cell(transactionRow, 5).Value = transaction.Quantity;
            transactionSheet.Cell(transactionRow, 6).Value = transaction.Reference ?? "";
            transactionSheet.Cell(transactionRow, 7).Value = transaction.CreatedBy ?? "";
            transactionRow++;
        }
        // Procurement
        var procurementSheet = workbook.Worksheets.Add("Procurement");
        procurementSheet.Cell(1, 1).Value = "Metric";
        procurementSheet.Cell(1, 2).Value = "Value";
        procurementSheet.Cell(2, 1).Value = "Purchase Orders";
        procurementSheet.Cell(2, 2).Value = procurement.TotalPurchaseOrders;
        procurementSheet.Cell(3, 1).Value = "Purchase Value";
        procurementSheet.Cell(3, 2).Value = procurement.TotalPurchaseValue;
        procurementSheet.Cell(4, 1).Value = "Draft";
        procurementSheet.Cell(4, 2).Value = procurement.DraftOrders;
        procurementSheet.Cell(5, 1).Value = "Ordered";
        procurementSheet.Cell(5, 2).Value = procurement.OrderedOrders;
        procurementSheet.Cell(6, 1).Value = "Partially Received";
        procurementSheet.Cell(6, 2).Value = procurement.PartiallyReceivedOrders;
        procurementSheet.Cell(7, 1).Value = "Received";
        procurementSheet.Cell(7, 2).Value = procurement.ReceivedOrders;
        procurementSheet.Cell(8, 1).Value = "Cancelled";
        procurementSheet.Cell(8, 2).Value = procurement.CancelledOrders;
        // Equipment
        var equipmentSheet = workbook.Worksheets.Add("Equipment");
        equipmentSheet.Cell(1, 1).Value = "Metric";
        equipmentSheet.Cell(1, 2).Value = "Value";
        equipmentSheet.Cell(2, 1).Value = "Total Equipment";
        equipmentSheet.Cell(2, 2).Value = equipment.TotalEquipment;
        equipmentSheet.Cell(3, 1).Value = "Available";
        equipmentSheet.Cell(3, 2).Value = equipment.Available;
        equipmentSheet.Cell(4, 1).Value = "In Use";
        equipmentSheet.Cell(4, 2).Value = equipment.InUse;
        equipmentSheet.Cell(5, 1).Value = "Maintenance";
        equipmentSheet.Cell(5, 2).Value = equipment.Maintenance;
        equipmentSheet.Cell(6, 1).Value = "Damaged";
        equipmentSheet.Cell(6, 2).Value = equipment.Damaged;
        equipmentSheet.Cell(7, 1).Value = "Retired";
        equipmentSheet.Cell(7, 2).Value = equipment.Retired;
        equipmentSheet.Cell(8, 1).Value = "Total Equipment Value";
        equipmentSheet.Cell(8, 2).Value = equipment.TotalEquipmentValue;
        // Projects
        var projectSheet = workbook.Worksheets.Add("Projects");
        projectSheet.Cell(1, 1).Value = "Metric";
        projectSheet.Cell(1, 2).Value = "Value";
        projectSheet.Cell(2, 1).Value = "Total Projects";
        projectSheet.Cell(2, 2).Value = projects.TotalProjects;
        projectSheet.Cell(3, 1).Value = "Active Projects";
        projectSheet.Cell(3, 2).Value = projects.ActiveProjects;
        projectSheet.Cell(4, 1).Value = "Inactive Projects";
        projectSheet.Cell(4, 2).Value = projects.InactiveProjects;
        // Formatting
        foreach (var worksheet in workbook.Worksheets)
        {
            worksheet.Row(1).Style.Font.Bold = true;
            worksheet.Columns().AdjustToContents();
        }
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return File(stream.ToArray(),"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"QuantumCount_Report_{DateTime.Now:yyyyMMdd_HHmm}.xlsx");
    }
    [HttpGet("export/pdf")]
    public async Task<IActionResult> ExportPdf([FromQuery] DateTime? fromDate = null,[FromQuery] DateTime? toDate = null)
    {
        var inventory = await _reportsService.GetInventorySummaryAsync();
        var procurement = await _reportsService.GetProcurementSummaryAsync(fromDate, toDate);
        var equipment = await _reportsService.GetEquipmentSummaryAsync();
        var projects = await _reportsService.GetProjectSummaryAsync();
        QuestPDF.Settings.License = LicenseType.Community;
        var pdf = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.Header().Text("Quantum Count").FontSize(24).Bold();
                page.Content().PaddingTop(20)
                    .Column(column =>
                    {
                        column.Item().Text("Reports & Analytics").FontSize(18).Bold();
                        column.Item().PaddingTop(5).Text($"Generated: {DateTime.Now:dd MMM yyyy HH:mm}");
                        column.Item().PaddingTop(20).Text("Inventory Summary").FontSize(15).Bold();
                        column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });
                                table.Cell().Text("Total Materials");
                                table.Cell().Text(inventory.TotalMaterials.ToString());
                                table.Cell().Text("Total Quantity");
                                table.Cell().Text(inventory.TotalQuantity.ToString("N2"));
                                table.Cell().Text("Inventory Value");
                                table.Cell().Text(inventory.TotalInventoryValue.ToString("N2"));
                                table.Cell().Text("Low Stock");
                                table.Cell().Text(inventory.LowStockItems.ToString());
                                table.Cell().Text("Out of Stock");
                                table.Cell().Text(inventory.OutOfStockItems.ToString());
                            });
                        column.Item().PaddingTop(20).Text("Procurement Summary").FontSize(15).Bold();
                        column.Item()
                            .Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });
                                table.Cell().Text("Purchase Orders");
                                table.Cell().Text(procurement.TotalPurchaseOrders.ToString());
                                table.Cell().Text("Purchase Value");
                                table.Cell().Text(procurement.TotalPurchaseValue.ToString("N2"));
                                table.Cell().Text("Draft");
                                table.Cell().Text(procurement.DraftOrders.ToString());
                                table.Cell().Text("Ordered");
                                table.Cell().Text(procurement.OrderedOrders.ToString());
                                table.Cell().Text("Received");
                                table.Cell().Text(procurement.ReceivedOrders.ToString());
                            });
                        column.Item()
                            .PaddingTop(20)
                            .Text("Equipment Summary")
                            .FontSize(15)
                            .Bold();

                        column.Item()
                            .Text(
                                $"Total: {equipment.TotalEquipment} | " +
                                $"Available: {equipment.Available} | " +
                                $"In Use: {equipment.InUse} | " +
                                $"Maintenance: {equipment.Maintenance} | " +
                                $"Damaged: {equipment.Damaged}");
                        column.Item().PaddingTop(20).Text("Project Summary").FontSize(15).Bold();
                        column.Item()
                            .Text(
                                $"Total: {projects.TotalProjects} | " +
                                $"Active: {projects.ActiveProjects} | " +
                                $"Inactive: {projects.InactiveProjects}");
                    });
                page.Footer().AlignCenter().Text("Quantum Count - Generated Report");
            });
        }).GeneratePdf();
        return File(pdf,"application/pdf",$"QuantumCount_Report_{DateTime.Now:yyyyMMdd_HHmm}.pdf");
    }
}