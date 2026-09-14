using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Quantum_Count.Models;
using Quantum_Count.Services;
namespace Quantum_Count.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PurchaseOrdersController : ControllerBase
{
    private readonly InventoryService _inventoryService;

    public PurchaseOrdersController(InventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }
    [HttpGet]
    public async Task<IActionResult> GetPurchaseOrders()
    {
        var purchaseOrders = await _inventoryService.GetPurchaseOrdersAsync();
        return Ok(purchaseOrders);
    }
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetPurchaseOrder(int id)
    {
        var purchaseOrder = await _inventoryService.GetPurchaseOrderAsync(id);
        if (purchaseOrder == null)
        {
            return NotFound(new
            {
                message = "Purchase order not found."
            });
        }
        return Ok(purchaseOrder);
    }
    [HttpPost]
    public async Task<IActionResult> CreatePurchaseOrder([FromBody] PurchaseOrder purchaseOrder)
    {
        var result = await _inventoryService.CreatePurchaseOrderAsync(purchaseOrder);
        if (!result.success)
        {
            return BadRequest(new
            {
                message = result.message
            });
        }
        return CreatedAtAction(nameof(GetPurchaseOrder),new { id = result.purchaseOrder!.Id },result.purchaseOrder);
    }
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdatePurchaseOrder(int id,[FromBody] PurchaseOrder purchaseOrder)
    {
        if (id != purchaseOrder.Id)
        {
            return BadRequest(new
            {
                message = "Purchase order ID in URL does not match request body."
            });
        }
        var result = await _inventoryService.UpdatePurchaseOrderAsync( purchaseOrder);
        if (!result.success)
        {
            return BadRequest(new
            {
                message = result.message
            });
        }
        return Ok(result.purchaseOrder);
    }
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletePurchaseOrder(int id)
    {
        var result = await _inventoryService.DeletePurchaseOrderAsync(id);
        if (!result.success)
        {
            return BadRequest(new
            {
                message = result.message
            });
        }
        return Ok(new
        {
            message = result.message
        });
    }
    [HttpPost("{purchaseOrderId:int}/items")]
    public async Task<IActionResult> AddPurchaseOrderItem(int purchaseOrderId,[FromBody] PurchaseOrderItem item)
    {
        if (purchaseOrderId != item.PurchaseOrderId)
        {
            return BadRequest(new
            {
                message = "Purchase order ID in URL does not match request body."
            });
        }
        var result = await _inventoryService.AddPurchaseOrderItem(item);
        if (!result.success)
        {
            return BadRequest(new
            {
                message = result.message
            });
        }
        return Ok(result.item);
    }
    [HttpDelete("items/{itemId:int}")]
    public async Task<IActionResult> RemovePurchaseOrderItem(
        int itemId)
    {
        var result = await _inventoryService.RemovePurchaseOrderItemAsync(itemId);
        if (!result.success)
        {
            return BadRequest(new
            {
                message = result.message
            });
        }
        return Ok(new
        {
            message = result.message
        });
    }
    [HttpPost("items/{itemId:int}/receive")]
    public async Task<IActionResult> ReceivePurchaseOrderItem(
        int itemId,
        [FromBody] ReceivePurchaseOrderItemRequest request)
    {
        var result =await _inventoryService.ReceivePurchaseOrderItemAsync(itemId,request.Quantity,User.Identity?.Name);
        if (!result.success)
        {
            return BadRequest(new
            {
                message = result.message
            });
        }
        return Ok(new
        {
            message = result.message,
            item = result.item
        });
    }
}
public class ReceivePurchaseOrderItemRequest
{
    public decimal Quantity { get; set; }
}