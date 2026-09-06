using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quantum_Count.Data;
using Quantum_Count.Models;

namespace Quantum_Count.Controllers;

[ApiController]
[Route("api/inventory-categories")]
public class InventoryCategoriesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public InventoryCategoriesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/inventory-categories
    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _context.InventoryCategories
            .OrderBy(c => c.Name)
            .ToListAsync();

        return Ok(categories);
    }

    // GET: api/inventory-categories/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCategory(int id)
    {
        var category = await _context.InventoryCategories
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
        {
            return NotFound(new
            {
                message = "Category not found."
            });
        }

        return Ok(category);
    }

    // POST: api/inventory-categories
    [HttpPost]
    public async Task<IActionResult> CreateCategory(
        [FromBody] InventoryCategory request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(new
            {
                message = "Category name is required."
            });
        }

        var exists = await _context.InventoryCategories
            .AnyAsync(c =>
                c.Name.ToLower() == request.Name.Trim().ToLower());

        if (exists)
        {
            return Conflict(new
            {
                message = "A category with this name already exists."
            });
        }

        var category = new InventoryCategory
        {
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            isActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.InventoryCategories.Add(category);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetCategory),
            new { id = category.Id },
            category);
    }

    // PUT: api/inventory-categories/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCategory(
        int id,
        [FromBody] InventoryCategory request)
    {
        var category = await _context.InventoryCategories
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
        {
            return NotFound(new
            {
                message = "Category not found."
            });
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(new
            {
                message = "Category name is required."
            });
        }

        var duplicate = await _context.InventoryCategories
            .AnyAsync(c =>
                c.Id != id &&
                c.Name.ToLower() == request.Name.Trim().ToLower());

        if (duplicate)
        {
            return Conflict(new
            {
                message = "Another category already uses this name."
            });
        }

        category.Name = request.Name.Trim();
        category.Description = request.Description?.Trim();

        await _context.SaveChangesAsync();

        return Ok(category);
    }

    // DELETE: api/inventory-categories/1
    // Soft delete
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await _context.InventoryCategories
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
        {
            return NotFound(new
            {
                message = "Category not found."
            });
        }

        category.isActive = false;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Category deleted successfully."
        });
    }

    // PUT: api/inventory-categories/1/restore
    [HttpPut("{id:int}/restore")]
    public async Task<IActionResult> RestoreCategory(int id)
    {
        var category = await _context.InventoryCategories
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
        {
            return NotFound(new
            {
                message = "Category not found."
            });
        }

        if (category.isActive)
        {
            return BadRequest(new
            {
                message = "Category is already active."
            });
        }

        category.isActive = true;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Category restored successfully."
        });
    }
}