using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Quantum_Count.Data;
using Quantum_Count.Models;
namespace Quantum_Count.Services;
public class SettingsService
{
    private readonly ApplicationDbContext _context;
    public SettingsService(ApplicationDbContext context)
    {
        _context = context;
    }
    public static event Action<ApplicationSettings>? SettingsUpdated;
    public async Task<ApplicationSettings> GetSettingsAsync()
    {
        var settings = await _context.ApplicationSettings.AsNoTracking().FirstOrDefaultAsync();
        if (settings != null)
            return settings;
        settings = new ApplicationSettings
        {
            OrganizationName = "Quantum Count",
            LowStockAlerts = true,
            AllowNegativeStock = false,
            PurchaseOrderNotifications = true,
            EquipmentMaintenanceNotifications = true,
            Theme = "Dark",
            CompactSidebar = false,
            UpdatedAt = DateTime.UtcNow
        };
        _context.ApplicationSettings.Add(settings);
        await _context.SaveChangesAsync();
        return settings;
    }
    public async Task<(bool success, string message, ApplicationSettings? settings)> UpdateSettingsAsync(ApplicationSettings request)
    {
        var settings = await _context.ApplicationSettings.FirstOrDefaultAsync();
        if (settings == null)
        {
            settings = new ApplicationSettings();
            _context.ApplicationSettings.Add(settings);
        }
        settings.OrganizationName = request.OrganizationName?.Trim() ?? "Quantum Count";
        settings.Currency = request.Currency;
        settings.TimeZone = request.TimeZone;
        settings.DateFormat = request.DateFormat;
        settings.LowStockAlerts = request.LowStockAlerts;
        settings.AllowNegativeStock = request.AllowNegativeStock;
        settings.DefaultUnit = request.DefaultUnit?.Trim() ?? "Piece";
        settings.PurchaseOrderNotifications = request.PurchaseOrderNotifications;
        settings.EquipmentMaintenanceNotifications = request.EquipmentMaintenanceNotifications;
        settings.Theme = request.Theme;
        settings.CompactSidebar = request.CompactSidebar;
        settings.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        SettingsUpdated?.Invoke(settings);
        return (true, "Settings updated successfully.", settings);
    }
}