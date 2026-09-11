using Microsoft.AspNetCore.Http.Features;
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
    //Category module
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
    //material
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
    //equipment
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
    // material transactions
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
    //equipment history
    public async Task<List<EquipmentHistory>> GetEquipmentHistoryAsync(int equipmentId)
    {
        return await _context.EquipmentHistories.Include(h => h.Equipment).Where(h => h.EquipmentId == equipmentId)
        .AsNoTracking().OrderByDescending(h => h.CreatedAt).ToListAsync();
    }
    public async Task<EquipmentHistory?> GetEquipmentHistoryByIdAsync(int id)
    {
        return await _context.EquipmentHistories.Include(h => h.Equipment).FirstOrDefaultAsync(h => h.Id == id);
    }
    public async Task<(bool success, string message, EquipmentHistory? history)>
    CreateEquipmentHistoryAsync( EquipmentHistory history, string? performedBy = null)
    {
        var equipment = await _context.Equipment.FirstOrDefaultAsync(e => e.Id == history.EquipmentId && e.isActive);
        if (equipment == null)
        {
            return ( false, "Equipment not found or is not active.",null );
        }
        if (string.IsNullOrWhiteSpace(history.ActivityType))
        {
            return ( false, "Activity type is required.", null );
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
            return ( false, "Invalid activity type.", null );
        }
        history.ActivityType = activityType;
        history.Reference = history.Reference?.Trim();
        history.Notes = history.Notes?.Trim();
        history.PerformedBy = performedBy;
        history.CreatedBy = performedBy ?? "System";
        history.CreatedAt = DateTime.UtcNow;
        _context.EquipmentHistories.Add(history);
        await _context.SaveChangesAsync();
        return (true,"Equipment history created successfully.", history );
    }
    //projects
    public async Task <List<Project>> GetProjectsAsync()
    {
        return await _context.Projects.Include(p => p.Tasks).Include(p => p.Materials).ThenInclude(pm => pm.Material).Include(p => p.Equipment).ThenInclude(pe => pe.Equipment).OrderByDescending(p => p.CreatedAt).ToListAsync();
    }
    public async Task <Project?> GetProjectAsync(int id)
    {
        return await _context.Projects.Include(p => p.Tasks).Include(p => p.Materials).ThenInclude(pm => pm.Material).Include(p => p.Equipment).ThenInclude(pe => pe.Equipment).FirstOrDefaultAsync(p => p.Id == id);
    }
    public async Task <(bool success,string message,Project? project)> CreateProjectAsync(Project project)
    {
        if (await _context.Projects.AnyAsync(p => p.ProjectCode == project.ProjectCode))
        {
            return (false, "A project with this project code already exists", null);
        }
        project.CreatedAt = DateTime.UtcNow;
        project.UpdatedAt = DateTime.UtcNow;
        project.isActive = true;
        if (string.IsNullOrWhiteSpace(project.Status))
            project.Status = "Planning";
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
        return (true, "Project created successfully", project);
    }
    public async Task <(bool success,string message,Project? project)> UpdateProjectAsync(Project project)
    {
        var existing = await _context.Projects.FirstOrDefaultAsync(p => p.Id == project.Id);
        if (existing == null)
        {
            return (false, "Project does not exists", null);
        }
        var duplicateCode = await _context.Projects.AnyAsync(p => p.ProjectCode == project.ProjectCode && p.Id != project.Id);
        if (duplicateCode)
        {
            return (false, "Another project uses same Project Code", null);
        }
        existing.ProjectCode = project.ProjectCode;
        existing.Name = project.Name;
        existing.Description = project.Description;
        existing.ClientOrDepartment = project.ClientOrDepartment;
        existing.StartDate = project.StartDate;
        existing.EndDate = project.EndDate;
        existing.Status = project.Status;
        existing.Budget = project.Budget;
        existing.isActive = project.isActive;
        existing.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return (true, $"Project updated successfully{project.Status}", project);
    }
    public async Task<(bool success, string message)> DeleteProjectAsync(int id)
    {
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id);
        if (project == null)
            return (false, "Project not found.");
        project.isActive = false;
        project.Status = "Cancelled";
        project.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return (true, "Project deactivated successfully.");
    }
    public async Task <(bool success,string message,ProjectMaterial? projectMaterial)> AddProjectMaterialAsync(ProjectMaterial request)
    {
        if (request.QuantityIssued <= 0)
            return (false, "Quantity issued must be greater than 0", null);
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == request.ProjectId);
        if (project == null)
            return (false, "Project not found", null);
        var material = await _context.Materials.FirstOrDefaultAsync(m => m.Id == request.MaterialId);
        if (material == null)
            return (false, "Material not found", null);
        if (!project.isActive)
            return (false, "Cannot add materials to an inactive project", null);
        if (material.Quantity < request.QuantityIssued)
            return (false, $"Insufficient stock.Available Quantity : {material.Quantity}", null);
        request.QuantityReturned = 0;
        request.QuantityUsed = request.QuantityIssued;
        request.IssuedAt = DateTime.UtcNow;
        request.isActive = true;
        material.Quantity -= request.QuantityIssued;
        _context.ProjectMaterials.Add(request);
        await _context.SaveChangesAsync();
        return (true, "Material issued to the project successfully", request);
    }
    //project materials
    public async Task<(bool success,string message)> ReturnProjectMaterialAsync(int ProjectMaterialId,decimal quantity)
    {
        if (quantity <= 0)
            return (false, "Quantity must be greater than 0.");
        var projectMaterial = await _context.ProjectMaterials.Include(pm => pm.Material).FirstOrDefaultAsync(pm => pm.Id == ProjectMaterialId);
        if (projectMaterial == null)
            return (false, "Project Material record not found.");
        if (!projectMaterial.isActive)
            return (false, "This Material allocation is no longer active");
        decimal AvailableToReturn = projectMaterial.QuantityIssued - projectMaterial.QuantityReturned;
        if (quantity > AvailableToReturn)
            return (false, $"Only {AvailableToReturn} units can be returned.");
        projectMaterial.QuantityReturned += quantity;
        projectMaterial.QuantityUsed = projectMaterial.QuantityIssued - projectMaterial.QuantityReturned;
        projectMaterial.ReturnedAt = DateTime.UtcNow;
        projectMaterial.Material!.Quantity += quantity;
        if (projectMaterial.QuantityReturned >= projectMaterial.QuantityIssued)
        {
            projectMaterial.isActive = false;
        }
        await _context.SaveChangesAsync();
        return (true, "Material returned Successfully");
    }
    public async Task<(bool success, string message)> RemoveProjectMaterialAsync(int ProjectMaterialId)
    {
        var projectMaterial = await _context.ProjectMaterials.Include(pm => pm.Material).FirstOrDefaultAsync(pm => pm.Id == ProjectMaterialId);
        if (projectMaterial == null)
            return (false, "Project Material record not found.");
        if (!projectMaterial.isActive)
            return (false, "This Material allocation is already inactive.");
        decimal remaining = projectMaterial.QuantityIssued - projectMaterial.QuantityReturned;
        if (remaining > 0)
        {
            projectMaterial.Material!.Quantity += remaining;
        }
        projectMaterial.QuantityReturned = projectMaterial.QuantityIssued;
        projectMaterial.isActive = false;
        projectMaterial.QuantityUsed = 0;
        projectMaterial.ReturnedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return (true, "Project material allocation closed successfully");
    }
    public async Task<List<ProjectMaterial>> GetProjectMaterialsAsync(int projectId)
    {
        return await _context.ProjectMaterials.Include(pm => pm.Material).Where(pm => pm.ProjectId == projectId)
        .OrderByDescending(pm => pm.IssuedAt).ToListAsync();
    }
    //project equipment
    public async Task <(bool success,string message,ProjectEquipment? projectEquipment)> AssignEquipmentToProjectAsync(ProjectEquipment request)
    {
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == request.ProjectId);
        if (project == null)
            return (false, "Project not found", null);
        var equipment = await _context.Equipment.FirstOrDefaultAsync(e => e.Id == request.EquipmentId);
        if (equipment == null)
            return (false, "Equipment not found", null);
        if (!project.isActive)
            return (false, "Cannot assign equipment to inactive project", null);
        if (!string.Equals(equipment.Status,"Available",StringComparison.OrdinalIgnoreCase))
            return (false, $"Equipment is currently {equipment.Status}.Cannot be assigned", null);
        bool alreadyAssigned = await _context.ProjectEquipment.AnyAsync(pe => pe.EquipmentId == request.EquipmentId && pe.IsActive);
        if (alreadyAssigned)
            return (false, "The Equipment is already assigned to a project", null);
        request.AssignedAt = DateTime.Now;
        request.ReturnedAt = null;
        request.IsActive = true;
        equipment.Status = "In Use";
        _context.ProjectEquipment.Add(request);
        await _context.SaveChangesAsync();
        return (true, "Equipment assigned Successfully", request);
    }
    public async Task<(bool success,string message)> ReturnEquipmentFromProjectAsync(int projectEquipmentId)
    {
        var projectEquipment = await _context.ProjectEquipment.Include(pe => pe.Equipment).FirstOrDefaultAsync(pe => pe.Id == projectEquipmentId);
        if (projectEquipment == null)
            return (false, "Project Equipment record not found.");
        if (!projectEquipment.IsActive)
            return (false, "The Equipment is already been returned.");
        projectEquipment.IsActive = false;
        projectEquipment.ReturnedAt = DateTime.UtcNow;
        projectEquipment.Equipment!.Status = "Available";
        await _context.SaveChangesAsync();
        return (true, "Equipment returned successfully.");
    }
    public async Task<List<ProjectEquipment>> GetProjectEquipmentAsync(int projectId)
    {
        return await _context.ProjectEquipment.Include(pe => pe.Equipment).Where(pe => pe.ProjectId == projectId)
       .OrderByDescending(pe => pe.AssignedAt).ToListAsync();
    }
    //Project tasks
    public async Task<List<ProjectTask>> GetProjectTasksAsync(int projectId)
    {
        return await _context.ProjectTasks.Include(t => t.Project).Where(t => t.ProjectId == projectId && t.IsActive).OrderBy(t => t.DueDate).ToListAsync();
    }
    public async Task<ProjectTask?> GetProjectTaskAsync(int id)
    {
        return await _context.ProjectTasks.Include(t => t.Project).FirstOrDefaultAsync(t => t.Id == id);
    }
    public async Task<(bool success,string message,ProjectTask? task)> CreateProjectTaskAsync(ProjectTask task)
    {
        var projectExists = await _context.Projects.AnyAsync(p => p.Id == task.ProjectId);
        if (!projectExists)
            return (false, "Project not found", null);
        if (task.Progress < 0 || task.Progress > 100)
            return (false, "Progress must be between 0 and 100", null);
        task.CreatedAt = DateTime.UtcNow;
        task.UpdatedAt = DateTime.UtcNow;
        task.IsActive = true;
        _context.ProjectTasks.Add(task);
        await _context.SaveChangesAsync();
        return (true, "Project task created successfully", task);
    }
    public async Task <(bool success,string message,ProjectTask? task)> UpdateProjectTaskAsync(ProjectTask task)
    {
        var existing = await _context.ProjectTasks.FirstOrDefaultAsync(t => t.Id == task.Id);
        if (existing == null)
            return (false, "Project task not found", null);
        if (task.Progress < 0 || task.Progress > 100)
            return (false, "Progress must be between 0 and 100", null);
        existing.Title = task.Title;
        existing.Description = task.Description;
        existing.Progress = task.Progress;
        existing.Status = task.Status;
        existing.StartDate = task.StartDate;
        existing.DueDate = task.DueDate;
        existing.IsActive = task.IsActive;
        existing.UpdatedAt = DateTime.UtcNow;
        if (existing.Progress == 100)
        {
            existing.Status = "Completed";
            existing.CompletedAt ??= DateTime.UtcNow;
        }
        else
        {
            existing.CompletedAt = null;
        }
        await _context.SaveChangesAsync();
        return (true, "Task Updated successfully", task);
    }
    public async Task<int> GetTaskProgressAsync(int projectId)
    {
        var progress = await _context.ProjectTasks.Where(t => t.ProjectId == projectId && t.IsActive)
        .Select(t => (int?)t.Progress).AverageAsync();
        return progress.HasValue ? (int)Math.Round(progress.Value) : 0;
    }
    //projects dashboard
    public async Task<object?> GetProjectDashboardAsync(int projectId)
    {
        var project = await GetProjectAsync(projectId);
        if (project == null)
            return null;
        var progress = await GetTaskProgressAsync(projectId);
        var activeMaterials = project.Materials.Where(m => m.isActive).ToList();
        var activeEquipment = project.Equipment.Where(e => e.IsActive).ToList();
        var activeTasks = project.Tasks.Where(t => t.IsActive).ToList();
        return new
        {
            Project = project,
            Progress = progress,
            TotalTasks = activeTasks.Count,
            CompletedTasks = activeTasks.Count(t => t.Progress == 100),
            ActiveMaterials = activeMaterials.Count,
            ActiveEquipment = activeEquipment.Count
        };
    }
    //supplier
    public async Task <List<Supplier>> GetSuppliersAsync()
    {
        return await _context.Suppliers.AsNoTracking().Where(s => s.IsActive).OrderBy(s => s.CompanyName).ToListAsync();
    }
    public async Task<Supplier?> GetSupplierAsync(int id)
    {
        return await _context.Suppliers.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id && s.IsActive);
    }
    public async Task<(bool success,string message,Supplier? supplier)> CreateSupplierAsync(Supplier supplier)
    {
        if (String.IsNullOrWhiteSpace(supplier.SupplierCode))
        {
            return (false, "Supplier code is mandatory", null);
        }
        if (String.IsNullOrWhiteSpace(supplier.CompanyName))
        {
            return (false, "Company Name is Required",null);
        }
        supplier.SupplierCode = supplier.SupplierCode.Trim();
        supplier.CompanyName = supplier.CompanyName.Trim();
        supplier.ContactPerson = supplier.ContactPerson?.Trim();
        supplier.Phone = supplier.Phone?.Trim();
        supplier.Email = supplier.Email?.Trim();
        supplier.Address = supplier.Address?.Trim();
        supplier.TaxNumber = supplier.TaxNumber?.Trim();
        bool exists = await _context.Suppliers.AnyAsync(s => s.SupplierCode == supplier.SupplierCode);
        if (exists)
        {
            return (false, "A Supplier with this Supplier Code Already exists.", null);
        }
        supplier.IsActive = true;
        supplier.CreatedAt = DateTime.UtcNow;
        supplier.UpdatedAt = null;
        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync();
        return (true, "Supplier added successfully", supplier);
    }
    public async Task <(bool success,string message,Supplier? supplier)> UpdateSupplierAsync(Supplier supplier)
    {
        var existing = await _context.Suppliers.FirstOrDefaultAsync(s => s.Id == supplier.Id && s.IsActive);
        if (existing == null)
        {
            return (false, "Supplier not found", null);
        }
        if (string.IsNullOrWhiteSpace(supplier.SupplierCode))
            return (false, "Supplier code is required.", null);
        if (string.IsNullOrWhiteSpace(supplier.CompanyName))
            return (false, "Company name is required.", null);
        string supplierCode = supplier.SupplierCode.Trim();
        bool duplicateCode = await _context.Suppliers.AnyAsync(s => s.SupplierCode == supplierCode && s.Id != supplier.Id);
        if (duplicateCode)
            return (false, "Another supplier uses this supplier code.", null);
        existing.SupplierCode = supplierCode;
        existing.CompanyName = supplier.CompanyName.Trim();
        existing.ContactPerson = supplier.ContactPerson;
        existing.Phone = supplier.Phone;
        existing.Email = supplier.Email;
        existing.Address = supplier.Address;
        existing.TaxNumber = supplier.TaxNumber;
        existing.IsActive = supplier.IsActive;
        existing.UpdatedAt = supplier.UpdatedAt;
        await _context.SaveChangesAsync();
        return (true, "Supplier updated successfully", supplier);
    }
    public async Task<(bool success,string message)> DeleteSupplierAsync(int id)
    {
        var supplier = await _context.Suppliers.FirstOrDefaultAsync(s => s.Id == id);
        if (supplier == null)
        {
            return (false, "Supplier not found");
        }
        bool hasPurchaseOrder = await _context.PurchaseOrders.AnyAsync(p => p.SupplierId == id);
        if (hasPurchaseOrder)
        {
            return (false, "Supplier Cannot be Deleted.There are purchase orders associated with it.");
        }
        supplier.IsActive = false;
        supplier.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return (true, "Supplier deactivated successfully");
    }
    //purchase orders
    public async Task<List<PurchaseOrder>> GetPurchaseOrdersAsync()
    {
        return await _context.PurchaseOrders.Include(p => p.Supplier).Include(p => p.Items)
            .ThenInclude(i => i.Material).AsNoTracking()
            .Where(p => p.IsActive).OrderByDescending(p => p.OrderDate).ToListAsync();
    }
    public async Task <PurchaseOrder?> GetPurchaseOrderAsync(int id)
    {
        return await _context.PurchaseOrders.Include(p => p.Supplier).Include(p => p.Items)
            .ThenInclude(i => i.Material).AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
    }
    public async Task <(bool success,string message,PurchaseOrder? purchaseOrder)> CreatePurchaseOrderAsync(PurchaseOrder purchaseOrder)
    {
        if (String.IsNullOrWhiteSpace(purchaseOrder.PONumber))
            return (false, "PO number is required", null);
        if (purchaseOrder.SupplierId <= 0)
            return (false, "Supplier is required", null);
        var supplier = await _context.Suppliers.FirstOrDefaultAsync(s => s.Id == purchaseOrder.SupplierId && s.IsActive);
        if (supplier == null)
            return (false, "Supplier record not found", null);
        purchaseOrder.PONumber = purchaseOrder.PONumber.Trim();
        purchaseOrder.Notes = purchaseOrder.Notes?.Trim();
        bool exists = await _context.PurchaseOrders.AnyAsync(p => p.PONumber == purchaseOrder.PONumber);
        if (exists)
            return (false, "An order with this PO Number already exists", null);
        purchaseOrder.Status = "Draft";
        purchaseOrder.IsActive = true;
        purchaseOrder.OrderDate = purchaseOrder.OrderDate == default ? DateTime.UtcNow : purchaseOrder.OrderDate;
        purchaseOrder.CreatedAt = DateTime.UtcNow;
        purchaseOrder.UpdatedAt = null;
        purchaseOrder.TotalAmount = 0;
        _context.PurchaseOrders.Add(purchaseOrder);
        await _context.SaveChangesAsync();
        return (true, "Purchase order created successfully", purchaseOrder);
    }
    public async Task <(bool success,string message,PurchaseOrder? purchaseOrder)> UpdatePurchaseOrderAsync(PurchaseOrder purchaseOrder)
    {
        var existing = await _context.PurchaseOrders.FirstOrDefaultAsync(p => p.Id == purchaseOrder.Id);
        if (existing == null)
            return (false, "Purchase order not found", null);
        if (existing.Status != "Draft")
            return (false, "Only draft purchase order can be updated.", null);
        var supplier = await _context.Suppliers.FirstOrDefaultAsync(s => s.Id == purchaseOrder.SupplierId && s.IsActive);
        if (supplier == null)
            return (false, "Supplier not found", null);
        string PoNumber = purchaseOrder.PONumber.Trim();
        bool exists = await _context.PurchaseOrders.AnyAsync(p => p.PONumber == PoNumber && p.Id != purchaseOrder.Id);
        if (exists)
            return (false, "A purchase order with same PO Number already exists", null);
        existing.PONumber = PoNumber;
        existing.SupplierId = purchaseOrder.SupplierId;
        existing.OrderDate = purchaseOrder.OrderDate;
        existing.ExpectedDeliveryDate = purchaseOrder.ExpectedDeliveryDate;
        existing.Notes = purchaseOrder.Notes?.Trim();
        existing.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return (true, "Purchase order updated successfully", purchaseOrder);
    }
    public async Task <(bool success,string message)> DeletePurchaseOrderAsync(int id)
    {
        var purchase = await _context.PurchaseOrders.FirstOrDefaultAsync(p => p.Id == id);
        if (purchase == null) return (false, "Purchase record not found");
        if (purchase.Status != "Draft")
            return (false, "Only draft purchases can be cancelled");
        purchase.IsActive = false;
        purchase.Status = "Cancelled";
        purchase.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return (true, "Purchased order successfully deleted");
    }
    //purchase order items
    public async Task<(bool success,string message,PurchaseOrderItem? item)> AddPurchaseOrderItem(PurchaseOrderItem item)
    {
        if (item.QuantityOrdered <= 0)
            return (false, "Quantity ordered must be more than 0.", null);
        if (item.UnitPrice < 0)
            return (false, "Unit price must be greater than 0",null);
        var purchaseOrder = await _context.PurchaseOrders.FirstOrDefaultAsync(p => p.Id == item.PurchaseOrderId && p.IsActive);
        if (purchaseOrder == null)
            return (false,"Purchased order not found", null);
        if (purchaseOrder.Status != "Draft")
            return (false, "Items can be only addded to a draft purchase order", null);
        var material = await _context.Materials.FirstOrDefaultAsync(m => m.Id == item.MaterialId && m.isActive);
        if (material == null) return (false, "Material not found",null);
        bool exists = await _context.PurchaseOrderItems.AnyAsync(i =>i.PurchaseOrderId == item.PurchaseOrderId && i.MaterialId == item.MaterialId && i.IsActive);
        if (exists) return (false, "This material already exists in purchase order",null);
        item.TotalPrice = item.QuantityOrdered * item.UnitPrice;
        item.QuantityReceived = 0;
        item.IsActive = true;
        item.CreatedAt = DateTime.UtcNow;
        item.Notes = item.Notes?.Trim();
        _context.PurchaseOrderItems.Add(item);
        purchaseOrder.TotalAmount += item.TotalPrice;
        purchaseOrder.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return (true, "Item successfully added to Purchase Order.", item);
    }
    public async Task <(bool success,string message)> RemovePurchaseOrderItemAsync(int id)
    {
        var item = await _context.PurchaseOrderItems.Include(i => i.PurchaseOrder).FirstOrDefaultAsync(i => i.Id == id);
        if (item == null)
            return (false, "Purchase order item not found");
        if (!item.IsActive)
            return (false, "Purchase order item is not active");
        if (item.PurchaseOrder == null)
            return (false, "Purchase order not found");
        if (item.PurchaseOrder.Status != "Draft")
            return (false, "Items can only be removed from a draft Purchase order only.");
        item.PurchaseOrder.TotalAmount -= item.TotalPrice;
        if (item.PurchaseOrder.TotalAmount < 0)
            item.PurchaseOrder.TotalAmount = 0;
        item.PurchaseOrder.UpdatedAt = DateTime.UtcNow;
        item.IsActive = false;
        await _context.SaveChangesAsync();
        return (true, "Purchase order item removed successfully.");
    }
    public async Task<(bool success, string message, PurchaseOrderItem? item)> ReceivePurchaseOrderItemAsync(int purchaseOrderItemId,decimal quantity,string? receivedBy = null) 
    {
        if (quantity <= 0)
            return (false, "Received quantity must be greater than 0.", null);
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var item = await _context.PurchaseOrderItems.Include(i => i.PurchaseOrder).Include(i => i.Material).FirstOrDefaultAsync(i => i.Id == purchaseOrderItemId && i.IsActive);
            if (item == null)
            {
                await transaction.RollbackAsync();
                return (false, "Purchase order item not found.", null);
            }
            if (item.PurchaseOrder == null)
            {
                await transaction.RollbackAsync();
                return (false, "Purchase order item not found", null);
            }
            if (item.Material == null)
            {
                await transaction.RollbackAsync();
                return (false, "Material item not found", null);
            }
            var purchaseOrder = item.PurchaseOrder;
            if (purchaseOrder.Status == "Cancelled")
            {
                await transaction.RollbackAsync();
                return (false, "Cannot receive items from a cancelled Purchase order", null);
            }
            decimal remaining = item.QuantityOrdered - item.QuantityReceived;
            if (quantity > remaining)
            {
                await transaction.RollbackAsync();
                return (false, $"Cannot receive {quantity}.Remaining Quantity is {remaining}", null);
            }
            item.Material.Quantity += quantity;
            item.Material.UpdatedAt = DateTime.UtcNow;
            item.QuantityReceived += quantity;
            var inventoryTransaction = new InventoryTransactions
            {
                MaterialId = item.MaterialId,
                Quantity = quantity,
                TransactionType = "Stock In",
                Reference = purchaseOrder.PONumber,
                Notes = $"Purchase receipt for {purchaseOrder.PONumber}",
                CreatedBy = receivedBy,
                CreatedAt = DateTime.UtcNow
            };
            _context.InventoryTransactions.Add(inventoryTransaction);
            var activeItem = await _context.PurchaseOrderItems.Where(i => i.PurchaseOrderId == purchaseOrder.Id && i.IsActive).ToListAsync();
            bool allReceived = activeItem.All(i => i.Id == item.Id 
            ? item.QuantityReceived >= item.QuantityOrdered : i.QuantityReceived >= i.QuantityOrdered);
            bool anyReceived = activeItem.Any(i => i.Id == item.Id ? item.QuantityReceived > 0 : i.QuantityReceived > 0);
            if (allReceived &&  activeItem.Count > 0)
            {
                purchaseOrder.Status = "Received";
            }
            else if (anyReceived)
            {
                purchaseOrder.Status = "Partially Received";
            }
            else
            {
                purchaseOrder.Status = "Ordered";
            }
            purchaseOrder.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return (true, "Purchase order item received successfully.", item);
        }
        catch
        {
            await transaction.RollbackAsync();
            return (false, "An error has occurred while receiving purchase order item", null);
        }
    }
}