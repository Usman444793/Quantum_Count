using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quantum_Count.Data;
using Quantum_Count.Models;
using Microsoft.AspNetCore.Authorization;
namespace Quantum_Count.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EquipmentController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public EquipmentController(ApplicationDbContext context)
    {
        _context = context;
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Equipment>>> GetEquipment()
    {
        var equipment = await _context.Equipment.Include(e => e.Category).Where(e => e.isActive)
            .AsNoTracking().OrderBy(e => e.Name).ToListAsync();
        return Ok(equipment);
    }
    [HttpGet("{id:int}")]
    public async Task <ActionResult <Equipment>> GetEquipmentById(int id)
    {
        var equipment = await _context.Equipment.Include(e => e.Category).AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        if (equipment == null)
        {
            return NotFound(new
            {
                message = "Equipment not found."
            });
        }
        return Ok(equipment);
    }
    [HttpPost]
    public async Task <ActionResult <Equipment>> CreateEquipment(Equipment equipment)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var category = await _context.InventoryCategories.FirstOrDefaultAsync(c => c.Id == equipment.CategoryId && c.isActive);
        if (category == null)
        {
            return BadRequest(new
            {
                message = "The Selected category does not exist or is inactive.",
                receivedCategoryId = equipment.CategoryId
            });
        }
        var codeExists = await _context.Equipment.AnyAsync(e => e.EquipmentCode == equipment.EquipmentCode);
        if (codeExists)
        {
            return BadRequest(new
            {
                message = "The Equipment Code already exists. Please use a different code."
            });
        }
        if (equipment.PurchasePrice < 0)
        {
            return BadRequest(new
            {
                message = "Purchase Price cannot be negative."
            });
        }
        if (!string.IsNullOrWhiteSpace(equipment.SerialNumber))
        {
            var serialExists = await _context.Equipment.AnyAsync(e => e.SerialNumber == equipment.SerialNumber);
            if (serialExists)
            {
                return BadRequest(new
                {
                    message = "The Serial Number already exists. Please use a different serial number."
                });
            }
        }
        equipment.CreatedAt = DateTime.UtcNow;
        equipment.UpdatedAt = null;
        equipment.isActive = true;
        _context.Equipment.Add(equipment);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetEquipmentById), new { id = equipment.Id }, equipment);
    }
    [HttpPut("{id:int}")]
    public async Task <ActionResult <Equipment>> UpdateEquipment(int id,Equipment equipment)
    {
        if (id != equipment.Id)
        {
            return BadRequest(new
            {
                message = "The Equipment ID in the URL does not match the ID in the request body."
            });
        }
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var existingEquipment = await _context.Equipment.FirstOrDefaultAsync(e => e.Id == id);
        if (existingEquipment == null)
        {
            return NotFound(new
            {
                message = "Equipment not found."
            });
        }
        var categoryExists = await _context.InventoryCategories.AnyAsync(c => c.Id == equipment.CategoryId && c.isActive);
        if (!categoryExists)
        {
            return BadRequest(new
            {
                message = "The Selected category does not exist or is inactive."
            });
        }
        var DuplicateCode = await _context.Equipment.AnyAsync(c => c.EquipmentCode == equipment.EquipmentCode && c.Id != id);
        if (DuplicateCode)
        {
            return BadRequest(new
            {
                message = "The Equipment Code already exists. Please use a different code."
            });
        }
        if (!string.IsNullOrWhiteSpace(equipment.SerialNumber))
        {
            var DuplicateSerial = await _context.Equipment.AnyAsync(e => e.SerialNumber == equipment.SerialNumber && e.Id != id);
            if (DuplicateSerial)
            {
                return BadRequest(new
                {
                    message =  "The Serial Number already exists. Please use a different serial number."
                });
            }
        }
        existingEquipment.EquipmentCode = equipment.EquipmentCode;
        existingEquipment.Name = equipment.Name;
        existingEquipment.SerialNumber = equipment.SerialNumber;
        existingEquipment.Brand = equipment.Brand;
        existingEquipment.Model = equipment.Model;
        existingEquipment.PurchaseDate = equipment.PurchaseDate;
        existingEquipment.PurchasePrice = equipment.PurchasePrice;
        existingEquipment.Status = equipment.Status;
        existingEquipment.Description = equipment.Description;
        existingEquipment.isActive = equipment.isActive;
        existingEquipment.CategoryId = equipment.CategoryId;
        existingEquipment.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }
    [HttpPut("{id:int}/restore")]
    public async Task<IActionResult> RestoreEquipment(int id)
    {
        var equipment = await _context.Equipment.FirstOrDefaultAsync(e => e.Id == id);
        if (equipment == null)
        {
            return NotFound(new
            {
                message = "Material not found."
            });
        }
        if (equipment.isActive)
        {
            return BadRequest(new
            {
                message = "Equipment is already active."
            });
        }
        equipment.isActive = true;
        equipment.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return Ok(new
        {
            message = "Equipment restored successfully."
        });
    }
    [HttpDelete("{id:int}")]
    public async Task <IActionResult> DeleteEquipment(int id) 
    {
        var equipment = await _context.Equipment.FirstOrDefaultAsync(e => e.Id == id);
        if (equipment == null)
        {
            return NotFound(new
            {
                message = "Equipment not found."
            });
        }
        equipment.isActive = false;
        equipment.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}