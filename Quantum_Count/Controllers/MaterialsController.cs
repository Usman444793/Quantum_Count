using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quantum_Count.Data;
using Quantum_Count.Models;
using System.Collections;
namespace Quantum_Count.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MaterialsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public MaterialsController(ApplicationDbContext context)
    {
        _context = context;
    }
    [HttpGet]
    public async Task <ActionResult <IEnumerable <Material>>> GetMaterials()
    {
        var materials = await _context.Materials.Include(e => e.Category).Where(m => m.isActive).AsNoTracking().OrderBy(m => m.Name).ToListAsync();
        return Ok(materials);
    }
    [HttpGet("{id:int}")]
    public async Task <ActionResult <Material>> GetMaterials(int id)
    {
        var material = await _context.Materials.Include(e => e.Category).AsNoTracking().OrderBy(m =>m.Name).FirstOrDefaultAsync(m => m.Id == id);
        if(material == null)
        {
            return NotFound();
        }
        return Ok(material);
    }
    [HttpPost]
    public async Task <ActionResult <Material>> CreateMaterial(Material material)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var categoryExists = await _context.InventoryCategories.AnyAsync(c => c.Id == material.CategoryId && c.isActive);
        if (!categoryExists)
        {
            return BadRequest(new
            {
                message = "The Selected Category does not exist or is inactive."
            });
        }
        var codeExists = await _context.Materials.AnyAsync(m => m.MaterialCode == material.MaterialCode);
        if (codeExists)
        {
            return Conflict("A material with the same Material Code already exists.");
        }
        material.CreatedAt = DateTime.UtcNow;
        material.UpdatedAt = DateTime.UtcNow;
        _context.Materials.Add(material);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetMaterials), new { id = material.Id }, material);
    }
    [HttpPut("{id:int}")]
    public async Task <IActionResult> UpdateMaterial(int id, Material material)
    {
        if (id != material.Id)
        { 
            return BadRequest(new
            {
                message = "Material Id Does not Match"
            });
        }
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var existingMaterial = await _context.Materials.FirstOrDefaultAsync(m => m.Id == id);
        if (existingMaterial == null)
        {
            return NotFound (new
            {
                message = "Material Not Found"
            });
        }
        var categoryExists = await _context.Materials.AnyAsync(m => m.CategoryId == material.CategoryId && material.isActive); 
        if (!categoryExists)
        {
            return BadRequest(new
            {
                message = "The Selected Category does not Exists or is Inactive."
            });
        }
        var DuplicateCodeExists = await _context.Materials.AnyAsync(m => m.MaterialCode == material.MaterialCode && m.Id != id);
        if (DuplicateCodeExists)
        {
            return Conflict(new
            {
                message = "A Material with the Same Material Code already exists."
            });
        }
        existingMaterial.MaterialCode = material.MaterialCode;
        existingMaterial.Name = material.Name;
        existingMaterial.Description = material.Description;
        existingMaterial.Unit = material.Unit;
        existingMaterial.Quantity = material.Quantity;
        existingMaterial.MinimumStockLevel = material.MinimumStockLevel;
        existingMaterial.MaximumStockLevel = material.MaximumStockLevel;
        existingMaterial.UnitPrice = material.UnitPrice;
        existingMaterial.isActive = material.isActive;
        existingMaterial.CategoryId = material.CategoryId;
        existingMaterial.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }
    [HttpPut("{id:int}/restore")]
    public async Task<IActionResult> RestoreMaterial(int id)
    {
        var material = await _context.Materials.FirstOrDefaultAsync(m => m.Id == id);
        if (material == null)
        {
            return NotFound(new
            {
                message = "Material not found."
            });
        }
        if (material.isActive)
        {
            return BadRequest(new
            {
                message = "Material is already active."
            });
        }
        material.isActive = true;
        material.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return Ok(new
        {
            message = "Material restored successfully."
        });
    }
    [HttpDelete("{id:int}")]
    public async Task <IActionResult> DeleteMaterial(int id)
    {
        var material = await _context.Materials.FirstOrDefaultAsync(m => m.Id == id);
        if (material == null)
        {
            return NotFound(new
            {
                message = "Material Not Found"
            });
        }
        material.isActive = false;
        material.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}