using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Quantum_Count.Data;
using Quantum_Count.Models;
namespace Quantum_Count.Services;
 public class InventoryService
 {
    private readonly ApplicationDbContext _context;
    public InventoryService(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<List<InventoryCategory>> GetCategoriesAsync()
    {
        return await _context.InventoryCategories
            .Include(c => c.Materials)
            .Include(c => c.Equipment)
            .Where(c => c.isActive)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }
    //material
    public async Task <List<Material>> GetMaterialsAsync()
    {
        return await _context.Materials.Include(m => m.Category).Where(m => m.isActive).AsNoTracking().OrderBy(m => m.Name).ToListAsync();
    }
    public async Task <bool> MaterialCodeExistsAsync(string MaterialCode, int? excludeId = null)
    {
        return await _context.Materials.AnyAsync(m => m.MaterialCode == MaterialCode && m.isActive && (!excludeId.HasValue || m.Id != excludeId.Value));
    }
    public async Task <Material?> GetMaterialAsync(int id)
    {
        return await _context.Materials.Include(m => m.Category).FirstOrDefaultAsync(m => m.Id == id);
    }
    public async Task <Material> CreateMaterialAsync(Material material)
    {
        material.CreatedAt = DateTime.UtcNow;
        material.UpdatedAt = null;
        material.isActive = true;
        _context.Materials.Add(material);
        _context.SaveChanges();
        return material;
    }
    public async Task <Material?> UpdateMaterialAsync(Material material)
    {
        var existingMaterial = await _context.Materials.FirstOrDefaultAsync(m => m.Id == material.Id);
        if (existingMaterial == null)
        {
            return null;
        }
        existingMaterial.MaterialCode = material.MaterialCode;
        existingMaterial.Name = material.Name;
        existingMaterial.Description = material.Description;
        existingMaterial.Unit = material.Unit;
        existingMaterial.Quantity = material.Quantity;
        existingMaterial.MinimumStockLevel = material.MinimumStockLevel;
        existingMaterial.MaximumStockLevel = material.MinimumStockLevel;
        existingMaterial.UnitPrice = material.UnitPrice;
        existingMaterial.CategoryId = material.CategoryId;
        existingMaterial.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return existingMaterial;
    }
    public async Task <bool> DeleteMaterialAsync(int id)
    {
        var material = await _context.Materials.FirstOrDefaultAsync(m => m.Id == id);
        if (material == null)
        {
            return false;
        }
        material.isActive = false;
        material.UpdatedAt = DateTime.UtcNow;
        return true;
    }
    //Equipment
    public async Task<List<Equipment>> GetEquipmentAsync()
    {
        return await _context.Equipment.Include(e => e.Category).Where(e => e.isActive).AsNoTracking().OrderBy(e => e.Name).ToListAsync();
    }
    public async Task <bool> EquipmentCodeExistsAsync(string equipmentCode, int? excludeId = null)
    {
        return await _context.Equipment.AnyAsync(e => e.EquipmentCode == equipmentCode && e.isActive && (!excludeId.HasValue || e.Id != excludeId.Value));
    }
    public async Task <bool> SerialNumberExistsAsync(string serialNumber, int? excludeId = null)
    {
        if (string.IsNullOrWhiteSpace(serialNumber))
        {
            return false;
        }
        return await _context.Equipment.AnyAsync(e => e.SerialNumber == serialNumber && e.isActive && (!excludeId.HasValue || e.Id != excludeId.Value));
    }
    public async Task <Equipment?> GetEquipmentAsync(int id)
    {
        return await _context.Equipment.Include(e => e.Category).FirstOrDefaultAsync(e => e.Id == id);
    }
    public async Task <Equipment> CreateEquipmentAsync(Equipment equipment)
    {
        equipment.CreatedAt = DateTime.UtcNow;
        equipment.UpdatedAt = null;
        equipment.isActive = true;
        _context.Equipment.Add(equipment);
        await _context.SaveChangesAsync();
        return equipment;
    }
    public async Task <Equipment?> UpdateEquipmentAsync(Equipment equipment)
    {
        var existing = await _context.Equipment.FirstOrDefaultAsync(e => e.Id == equipment.Id);
        if (existing == null)
        {
            return null;
        }
        existing.EquipmentCode = equipment.EquipmentCode;
        existing.Name = equipment.Name;
        existing.SerialNumber = equipment.SerialNumber;
        existing.Brand = equipment.Brand;
        existing.Model = equipment.Model;
        existing.PurchaseDate = equipment.PurchaseDate;
        existing.PurchasePrice = equipment.PurchasePrice;
        existing.Status = equipment.Status;
        existing.Description = equipment.Description;
        existing.CategoryId = equipment.CategoryId;
        existing.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return existing;
    }
    public async Task <bool> DeleteEquipmentAsync(int id)
    {
        var equipment = await _context.Equipment.FirstOrDefaultAsync(e => e.Id == id);
        if(equipment == null)
        {
            return false;
        }
        equipment.isActive = false;
        equipment.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<InventoryCategory?> GetCategoryAsync(int id)
    {
        return await _context.InventoryCategories
            .FirstOrDefaultAsync(c => c.Id == id);
    }
    public async Task<InventoryCategory> CreateCategoryAsync(
    InventoryCategory category)
    {
        category.Name = category.Name.Trim();
        category.Description = category.Description?.Trim();
        category.isActive = true;
        category.CreatedAt = DateTime.UtcNow;

        _context.InventoryCategories.Add(category);

        await _context.SaveChangesAsync();

        return category;
    }
    public async Task<InventoryCategory?> UpdateCategoryAsync(
    InventoryCategory category)
    {
        var existing = await _context.InventoryCategories
            .FirstOrDefaultAsync(c => c.Id == category.Id);

        if (existing == null)
            return null;

        existing.Name = category.Name.Trim();
        existing.Description = category.Description?.Trim();

        await _context.SaveChangesAsync();

        return existing;
    }
    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var category = await _context.InventoryCategories
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
            return false;

        category.isActive = false;

        await _context.SaveChangesAsync();

        return true;
    }
}