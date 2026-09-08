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
        return await _context.InventoryCategories.AsNoTracking().Where(c => c.isActive).OrderBy(c => c.Name).ToListAsync();
    }
    public async Task<List<InventoryCategory>> GetCategoriesWithCountsAsync()
    {
        return await _context.InventoryCategories.AsNoTracking()
            .Include(c => c.Materials.Where(m => m.isActive))
            .Include(c => c.Equipment.Where(e => e.isActive))
            .Where(c => c.isActive)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }
    public async Task<InventoryCategory?> GetCategoryAsync(int id)
    {
        return await _context.InventoryCategories.FirstOrDefaultAsync(c => c.Id == id);
    }
    public async Task<InventoryCategory> CreateCategoryAsync(InventoryCategory category)
    {
        category.Name = category.Name.Trim();
        category.Description = category.Description?.Trim();
        category.isActive = true;
        category.CreatedAt = DateTime.UtcNow;
        _context.InventoryCategories.Add(category);
        await _context.SaveChangesAsync();
        return category;
    }
    public async Task<InventoryCategory?> UpdateCategoryAsync(InventoryCategory category)
    {
        var existing = await _context.InventoryCategories.FirstOrDefaultAsync(c => c.Id == category.Id);
        if (existing == null)
            return null;
        existing.Name = category.Name.Trim();
        existing.Description = category.Description?.Trim();
        await _context.SaveChangesAsync();
        return existing;
    }
    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var category = await _context.InventoryCategories.FirstOrDefaultAsync(c => c.Id == id);
        if (category == null)
            return false;
        category.isActive = false;
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<List<Material>> GetMaterialsAsync()
    {
        return await _context.Materials
            .Include(m => m.Category)
            .Where(m => m.isActive)
            .AsNoTracking()
            .OrderBy(m => m.Name)
            .ToListAsync();
    }
    public async Task<bool> MaterialCodeExistsAsync(string materialCode,int? excludeId = null)
    {
        return await _context.Materials
            .AnyAsync(m =>
                m.MaterialCode == materialCode &&
                m.isActive &&
                (!excludeId.HasValue ||
                 m.Id != excludeId.Value));
    }
    public async Task<Material?> GetMaterialAsync(int id)
    {
        return await _context.Materials.Include(m => m.Category).FirstOrDefaultAsync(m => m.Id == id);
    }
    public async Task<Material> CreateMaterialAsync(Material material)
    {
        material.MaterialCode = material.MaterialCode.Trim();
        material.Name = material.Name.Trim();
        material.Unit = material.Unit.Trim();
        material.Description = material.Description?.Trim();
        material.CreatedAt = DateTime.UtcNow;
        material.UpdatedAt = null;
        material.isActive = true;
        _context.Materials.Add(material);
        await _context.SaveChangesAsync();
        return material;
    }
    public async Task<Material?> UpdateMaterialAsync(Material material)
    {
        var existingMaterial =await _context.Materials.FirstOrDefaultAsync(m => m.Id == material.Id);
        if (existingMaterial == null)
            return null;
        existingMaterial.MaterialCode =material.MaterialCode.Trim();
        existingMaterial.Name =material.Name.Trim();
        existingMaterial.Description = material.Description?.Trim();
        existingMaterial.Unit = material.Unit.Trim();
        existingMaterial.Quantity = material.Quantity;
        existingMaterial.MinimumStockLevel = material.MinimumStockLevel;
        existingMaterial.MaximumStockLevel = material.MaximumStockLevel;
        existingMaterial.UnitPrice = material.UnitPrice;
        existingMaterial.CategoryId = material.CategoryId;
        existingMaterial.UpdatedAt =DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return existingMaterial;
    }
    public async Task<bool> DeleteMaterialAsync(int id)
    {
        var material = await _context.Materials.FirstOrDefaultAsync(m => m.Id == id);
        if (material == null)
            return false;
        material.isActive = false;
        material.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<List<Equipment>> GetEquipmentAsync()
    {
        return await _context.Equipment
            .Include(e => e.Category)
            .Where(e => e.isActive)
            .AsNoTracking()
            .OrderBy(e => e.Name)
            .ToListAsync();
    }
    public async Task<bool> EquipmentCodeExistsAsync(string equipmentCode,int? excludeId = null)
    {
        return await _context.Equipment.AnyAsync(e => e.EquipmentCode == equipmentCode && e.isActive &&
        (!excludeId.HasValue || e.Id != excludeId.Value));
    }
    public async Task<bool> SerialNumberExistsAsync(string serialNumber,int? excludeId = null)
    {
        if (string.IsNullOrWhiteSpace(serialNumber))
            return false;
        return await _context.Equipment.AnyAsync(e => e.SerialNumber == serialNumber && e.isActive &&
        (!excludeId.HasValue || e.Id != excludeId.Value));
    }
    public async Task<Equipment?> GetEquipmentAsync(int id)
    {
        return await _context.Equipment.Include(e => e.Category).AsNoTracking().FirstOrDefaultAsync(e => e.Id == id && e.isActive);
    }
    public async Task<Equipment> CreateEquipmentAsync(Equipment equipment)
    {
        equipment.EquipmentCode = equipment.EquipmentCode.Trim();
        equipment.Name = equipment.Name.Trim();
        equipment.SerialNumber = equipment.SerialNumber?.Trim();
        equipment.Brand = equipment.Brand?.Trim();
        equipment.Model = equipment.Model?.Trim();
        equipment.Description = equipment.Description?.Trim();
        equipment.CreatedAt = DateTime.UtcNow;
        equipment.UpdatedAt = null;
        equipment.isActive = true;
        _context.Equipment.Add(equipment);
        await _context.SaveChangesAsync();
        return equipment;
    }
    public async Task<Equipment?> UpdateEquipmentAsync(Equipment equipment)
    {
        var existing = await _context.Equipment.FirstOrDefaultAsync(e => e.Id == equipment.Id);
        if (existing == null)
            return null;
        existing.EquipmentCode = equipment.EquipmentCode.Trim();
        existing.Name = equipment.Name.Trim();
        existing.SerialNumber = equipment.SerialNumber?.Trim();
        existing.Brand = equipment.Brand?.Trim();
        existing.Model = equipment.Model?.Trim();
        existing.PurchaseDate = equipment.PurchaseDate;
        existing.PurchasePrice = equipment.PurchasePrice;
        existing.Status = equipment.Status;
        existing.Description = equipment.Description?.Trim();
        existing.CategoryId = equipment.CategoryId;
        existing.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return existing;
    }
    public async Task<bool> DeleteEquipmentAsync(int id)
    {
        var equipment = await _context.Equipment.FirstOrDefaultAsync(e => e.Id == id);
        if (equipment == null)
            return false;
        equipment.isActive = false;
        equipment.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<List<InventoryTransactions>> GetTransactionsAsync()
    {
        return await _context.InventoryTransactions.Include(t => t.Material).AsNoTracking().OrderByDescending(t => t.CreatedAt).ToListAsync();
    }
    public async Task<List<InventoryTransactions>> GetMaterialTransactionsAsync(int materialId)
    {
        return await _context.InventoryTransactions.Include(t => t.Material).Where(t => t.MaterialId == materialId).AsNoTracking().OrderByDescending(t => t.CreatedAt).ToListAsync();
    }
    public async Task<InventoryTransactions?> GetTransactionAsync(int id)
    {
        return await _context.InventoryTransactions.Include(t => t.Material).FirstOrDefaultAsync(t => t.Id == id);
    }
    public async Task<(bool success,string message,InventoryTransactions? transaction)> CreateTransactionAsync(InventoryTransactions transaction,string? createdBy = null)
    {
        var material = await _context.Materials.FirstOrDefaultAsync(m => m.Id == transaction.MaterialId && m.isActive);
        if (material == null) 
        {
            return (false,"Material not found or is not active",null);
        }
        if (transaction.Quantity <= 0)
        {
            return (false, "Quantity must be grater than 0", null);
        }
        var transactionType = transaction.TransactionType.Trim();
        decimal StockChange;
        switch (transactionType) 
        {
            case "Stock In":
            case "Return":
                StockChange =transaction.Quantity; 
                break;
            case "Stock Out":
            case "Damaged":
                StockChange = -transaction.Quantity;
                break;
            case "Adjustment":
                StockChange = transaction.Quantity;
                break;
            default:
                return (false, "Invalid Transaction type", null);
        }
        var newQuantity = material.Quantity + StockChange;
        if (newQuantity < 0)
        {
            return (false, $"Insufficient stock. Available Quantity: {material.Quantity}", null);
        }
        transaction.TransactionType = transactionType;
        transaction.Reference = transaction.Reference?.Trim();
        transaction.Notes = transaction.Notes?.Trim();
        transaction.CreatedBy = createdBy;
        transaction.CreatedAt = DateTime.UtcNow;
        material.Quantity = newQuantity;
        material.UpdatedAt = DateTime.UtcNow;
        _context.InventoryTransactions.Add(transaction);
        await _context.SaveChangesAsync();
        return (true, "Transaction created successfully", transaction);
    }
    public async Task<List<EquipmentHistory>> GetEquipmentHistoryAsync(
    int equipmentId)
    {
        return await _context.EquipmentHistories
            .Include(h => h.Equipment)
            .Where(h => h.EquipmentId == equipmentId)
            .AsNoTracking()
            .OrderByDescending(h => h.CreatedAt)
            .ToListAsync();
    }
    public async Task<EquipmentHistory?> GetEquipmentHistoryByIdAsync(int id)
    {
        return await _context.EquipmentHistories
            .Include(h => h.Equipment)
            .FirstOrDefaultAsync(h => h.Id == id);
    }
    public async Task<(bool success, string message, EquipmentHistory? history)>
    CreateEquipmentHistoryAsync(
        EquipmentHistory history,
        string? performedBy = null)
    {
        var equipment = await _context.Equipment
            .FirstOrDefaultAsync(e =>
                e.Id == history.EquipmentId &&
                e.isActive);

        if (equipment == null)
        {
            return (
                false,
                "Equipment not found or is not active.",
                null
            );
        }

        if (string.IsNullOrWhiteSpace(history.ActivityType))
        {
            return (
                false,
                "Activity type is required.",
                null
            );
        }

        var validTypes = new[]
        {
        "Assignment",
        "Return",
        "Maintenance",
        "Repair",
        "Inspection",
        "Status Change"
    };

        var activityType = history.ActivityType.Trim();

        if (!validTypes.Contains(activityType))
        {
            return (
                false,
                "Invalid activity type.",
                null
            );
        }

        history.ActivityType = activityType;
        history.Reference = history.Reference?.Trim();
        history.Notes = history.Notes?.Trim();
        history.PerformedBy = performedBy;
        history.CreatedBy = performedBy ?? "System";
        history.CreatedAt = DateTime.UtcNow;

        _context.EquipmentHistories.Add(history);

        await _context.SaveChangesAsync();

        return (
            true,
            "Equipment history created successfully.",
            history
        );
    }
}