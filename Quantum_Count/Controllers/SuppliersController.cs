using Microsoft.AspNetCore.Mvc;
using Quantum_Count.Models;
using Quantum_Count.Services;
namespace Quantum_Count.Controllers;
[ApiController]
[Route("api/[controller]")]
public class SuppliersController : ControllerBase
{
    private readonly InventoryService _inventoryService;
    public SuppliersController(InventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }
    [HttpGet]
    public async Task<IActionResult> GetSuppliers()
    {
        var suppliers = await _inventoryService.GetSuppliersAsync();
        return Ok(suppliers);
    }
    [HttpGet("{id:int}")]
    public async Task<IActionResult?> GetSupplier(int id)
    {
        var supplier = await _inventoryService.GetSupplierAsync(id);
        if (supplier == null)
        {
            return NotFound(new
            {
                message = "Supplier record not found"
            });
        }
        return Ok(supplier);
    }
    [HttpPost]
    public async Task<IActionResult> CreateSupplier(Supplier supplier)
    {
        var result = await _inventoryService.CreateSupplierAsync(supplier);
        if (!result.success)
        {
            return BadRequest(new
            {
                message = result.message
            });
        }
        return CreatedAtAction(nameof(GetSupplier), new { id = result.supplier!.Id }, supplier);
    }
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateSupplier(int id,[FromBody]Supplier supplier)
    {
        if (id != supplier.Id)
        {
            return BadRequest(new
            {
                message = "Supplier Id in URl does not match request body."
            });
        }
        var result = await _inventoryService.UpdateSupplierAsync(supplier);
        if (!result.success) 
        {
            return BadRequest(new
            {
                message = result.message
            });
        }
        return Ok(result.supplier);
    }
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteSupplier(int id)
    {
        var result = await _inventoryService.DeleteSupplierAsync(id);
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
}