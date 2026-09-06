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
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Material>()
       .HasOne(m => m.Category).WithMany(c => c.Materials).HasForeignKey(m => m.CategoryId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Equipment>()
        .HasOne(e => e.Category).WithMany(c => c.Equipment).HasForeignKey(e => e.CategoryId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Material>().Property(m => m.UnitPrice).HasPrecision(18, 2);
        builder.Entity<Material>().Property(m => m.Quantity).HasPrecision(18, 2);
        builder.Entity<Material>().Property(m => m.MinimumStockLevel).HasPrecision(18, 2);
        builder.Entity<Material>().Property(m => m.MaximumStockLevel).HasPrecision(18, 2);
        builder.Entity<Equipment>().Property(e => e.PurchasePrice).HasPrecision(18, 2);
        builder.Entity<Material>().HasIndex(m => m.MaterialCode).IsUnique();
        builder.Entity<Equipment>().HasIndex(e => e.EquipmentCode).IsUnique();
        builder.Entity<Equipment>().HasIndex(e => e.SerialNumber).IsUnique().HasFilter($"[SerialNumber] IS NOT NULL");
    }
}