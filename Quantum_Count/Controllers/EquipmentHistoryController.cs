using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quantum_Count.Models;
using Quantum_Count.Services;
namespace Quantum_Count.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EquipmentHistoryController : ControllerBase
{
    private readonly InventoryService _inventoryService;

    public EquipmentHistoryController(InventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }
    [HttpGet("equipment/{equipmentId:int}")]
    public async Task<IActionResult> GetEquipmentHistory(int equipmentId)
    {
        var history = await _inventoryService.GetEquipmentHistoryAsync(equipmentId);
        return Ok(history);
    }
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetEquipmentHistoryById(int id)
    {
        var history = await _inventoryService.GetEquipmentHistoryByIdAsync(id);
        if (history == null)
        {
            return NotFound(new { message = "Equipment history record not found." });
        }
        return Ok(history);
    }
    [HttpPost]
    public async Task<IActionResult> CreateEquipmentHistory([FromBody] EquipmentHistory history)
    {
        var result = await _inventoryService.CreateEquipmentHistoryAsync(history, User.Identity?.Name);
        if (!result.success)
        {
            return BadRequest(new { message = result.message });
        }
        return CreatedAtAction(nameof(GetEquipmentHistoryById), new { id = result.history?.Id }, result.history);
    }
}