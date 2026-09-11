using Microsoft.AspNetCore.Mvc;
using Quantum_Count.Services;
using Microsoft.AspNetCore.Authorization;
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
}