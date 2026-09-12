using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Quantum_Count.Models;
namespace Quantum_Count.Data;
public class ApplicationDbContext : IdentityDbContext<ApplicationUsers>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    public DbSet<Material> Materials { get; set; }
    public DbSet<Equipment> Equipment { get; set; }
    public DbSet<InventoryCategory> InventoryCategories { get; set; }
    public DbSet<InventoryTransactions> InventoryTransactions { get; set; }
    public DbSet<EquipmentHistory> EquipmentHistories { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectMaterial> ProjectMaterials { get; set; }
    public DbSet<ProjectEquipment> ProjectEquipment { get; set; }
    public DbSet<ProjectTask> ProjectTasks { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
    public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }
    public DbSet<ApplicationSettings> ApplicationSettings { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Material>().HasOne(m => m.Category).WithMany(c => c.Materials)
            .HasForeignKey(m => m.CategoryId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Equipment>().HasOne(e => e.Category).WithMany(c => c.Equipment)
            .HasForeignKey(e => e.CategoryId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Material>().Property(m => m.UnitPrice).HasPrecision(18, 2);
        builder.Entity<Material>().Property(m => m.Quantity).HasPrecision(18, 2);
        builder.Entity<Material>().Property(m => m.MinimumStockLevel).HasPrecision(18, 2);
        builder.Entity<Material>().Property(m => m.MaximumStockLevel).HasPrecision(18, 2);
        builder.Entity<Equipment>().Property(e => e.PurchasePrice).HasPrecision(18, 2);
        builder.Entity<Material>().HasIndex(m => m.MaterialCode).IsUnique();
        builder.Entity<Equipment>().HasIndex(e => e.EquipmentCode).IsUnique();
        builder.Entity<Equipment>().HasIndex(e => e.SerialNumber).IsUnique().HasFilter($"[SerialNumber] IS NOT NULL");
        builder.Entity<InventoryTransactions>().HasOne(t => t.Material).WithMany()
            .HasForeignKey(t => t.MaterialId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<InventoryTransactions>().Property(t => t.Quantity).HasPrecision(18, 2);
        builder.Entity<EquipmentHistory>().HasOne(a => a.Equipment).WithMany()
            .HasForeignKey(a => a.EquipmentId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<ProjectMaterial>().HasOne(pm => pm.Project).WithMany(pm => pm.Materials)
            .HasForeignKey(pm => pm.ProjectId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<ProjectMaterial>().HasOne(pm => pm.Material).WithMany()
            .HasForeignKey(pm => pm.MaterialId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<ProjectEquipment>().HasOne(pe => pe.Project).WithMany(pe => pe.Equipment)
            .HasForeignKey(pe => pe.ProjectId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<ProjectEquipment>().HasOne(pe => pe.Equipment).WithMany()
            .HasForeignKey(pe => pe.EquipmentId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<ProjectTask>().HasOne(pt => pt.Project).WithMany(pt => pt.Tasks)
            .HasForeignKey(pt => pt.ProjectId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<Project>().Property(p => p.Budget).HasPrecision(18, 2);
        builder.Entity<ProjectMaterial>().Property(pm => pm.QuantityIssued).HasPrecision(18, 2);
        builder.Entity<ProjectMaterial>().Property(pm => pm.QuantityReturned).HasPrecision(18, 2);
        builder.Entity<ProjectMaterial>().Property(pm => pm.QuantityUsed).HasPrecision(18, 2);
        builder.Entity<Project>().HasIndex(p => p.ProjectCode).IsUnique();
        builder.Entity<Supplier>().HasIndex(s => s.SupplierCode).IsUnique();
        builder.Entity<PurchaseOrder>().HasIndex(p => p.PONumber).IsUnique();
        builder.Entity<PurchaseOrder>().HasOne(p => p.Supplier).WithMany(s => s.PurchaseOrders)
            .HasForeignKey(p => p.SupplierId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<PurchaseOrderItem>().HasOne(i => i.PurchaseOrder).WithMany(p => p.Items)
            .HasForeignKey(i => i.PurchaseOrderId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<PurchaseOrderItem>().HasOne(i => i.Material).WithMany()
            .HasForeignKey(i => i.MaterialId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<PurchaseOrder>().Property(p => p.TotalAmount).HasPrecision(18, 2);
        builder.Entity<PurchaseOrderItem>().Property(i => i.QuantityOrdered).HasPrecision(18, 2);
        builder.Entity<PurchaseOrderItem>().Property(i => i.QuantityReceived).HasPrecision(18, 2);
        builder.Entity<PurchaseOrderItem>().Property(i => i.UnitPrice).HasPrecision(18, 2);
        builder.Entity<PurchaseOrderItem>().Property(i => i.TotalPrice).HasPrecision(18, 2);
        builder.Entity<ApplicationSettings>().Property(s => s.OrganizationName).HasMaxLength(150);
        builder.Entity<ApplicationSettings>().Property(s => s.Currency).HasMaxLength(10);
        builder.Entity<ApplicationSettings>().Property(s => s.TimeZone).HasMaxLength(100);
        builder.Entity<ApplicationSettings>().Property(s => s.DateFormat).HasMaxLength(30);
        builder.Entity<ApplicationSettings>().Property(s => s.DefaultUnit).HasMaxLength(30);
        builder.Entity<ApplicationSettings>().Property(s => s.Theme).HasMaxLength(20);
    }
}