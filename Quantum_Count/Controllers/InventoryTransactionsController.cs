using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quantum_Count.Models;
using Quantum_Count.Services;
namespace Quantum_Count.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InventoryTransactionsController : ControllerBase
{
    private readonly InventoryService _inventoryService;
    public InventoryTransactionsController(InventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }
    [HttpGet]
    public async Task <IActionResult> GetTransactions()
    {
        var transactions = await _inventoryService.GetTransactionsAsync();
        return Ok(transactions);
    }
    [HttpGet("material/{materialId:int}")]
    public async Task <IActionResult> GetMaterialTransaction(int materialId)
    {
        var transaction = await _inventoryService.GetMaterialTransactionsAsync(materialId);
        return Ok(transaction);
    }
    [HttpGet("{id:int}")]
    public async Task <IActionResult> GetTransaction(int id)
    {
        var transaction = await _inventoryService.GetTransactionAsync(id);
        if (transaction == null)
        {
            return BadRequest(new
            {
                message = "Transaction not found."
            });
        }
        return Ok(transaction);
    }
    [HttpPost]
    public async Task <IActionResult> CreateTransaction([FromBody] InventoryTransactions transaction)
    {
        var result = await _inventoryService.CreateTransactionAsync(transaction, User.Identity?.Name);
        if (!result.success)
        {
            return BadRequest(new
            {
                message = result.message,
            });
        }
        return CreatedAtAction(nameof(GetTransaction), new {id = result.transaction!.Id},result.transaction);
    }
}