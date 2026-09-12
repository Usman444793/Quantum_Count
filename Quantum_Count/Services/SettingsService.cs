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

    public async Task<ApplicationSettings> GetSettingsAsync()
    {
        var settings = await _context.ApplicationSettings
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (settings != null)
            return settings;

        settings = new ApplicationSettings
        {
            OrganizationName = "Quantum Count",
            Currency = "PKR",
            TimeZone = "Pakistan Standard Time",
            DateFormat = "dd/MM/yyyy",
            LowStockAlerts = true,
            AllowNegativeStock = false,
            DefaultUnit = "Piece",
            PurchaseOrderNotifications = true,
            EquipmentMaintenanceNotifications = true,
            Theme = "Light",
            CompactSidebar = false,
            UpdatedAt = DateTime.UtcNow
        };

        _context.ApplicationSettings.Add(settings);
        await _context.SaveChangesAsync();

        return settings;
    }

    public async Task<(bool success, string message, ApplicationSettings? settings)>
        UpdateSettingsAsync(ApplicationSettings request)
    {
        var settings = await _context.ApplicationSettings
            .FirstOrDefaultAsync();

        if (settings == null)
        {
            settings = new ApplicationSettings
            {
                OrganizationName = request.OrganizationName?.Trim() ?? "Quantum Count",
                Currency = request.Currency,
                TimeZone = request.TimeZone,
                DateFormat = request.DateFormat,
                LowStockAlerts = request.LowStockAlerts,
                AllowNegativeStock = request.AllowNegativeStock,
                DefaultUnit = request.DefaultUnit?.Trim() ?? "Piece",
                PurchaseOrderNotifications = request.PurchaseOrderNotifications,
                EquipmentMaintenanceNotifications =
                    request.EquipmentMaintenanceNotifications,
                Theme = request.Theme,
                CompactSidebar = request.CompactSidebar,
                UpdatedAt = DateTime.UtcNow
            };

            _context.ApplicationSettings.Add(settings);
        }
        else
        {
            settings.OrganizationName =
                request.OrganizationName?.Trim() ?? settings.OrganizationName;

            settings.Currency = request.Currency;
            settings.TimeZone = request.TimeZone;
            settings.DateFormat = request.DateFormat;

            settings.LowStockAlerts = request.LowStockAlerts;
            settings.AllowNegativeStock = request.AllowNegativeStock;
            settings.DefaultUnit =
                request.DefaultUnit?.Trim() ?? settings.DefaultUnit;

            settings.PurchaseOrderNotifications =
                request.PurchaseOrderNotifications;

            settings.EquipmentMaintenanceNotifications =
                request.EquipmentMaintenanceNotifications;

            settings.Theme = request.Theme;
            settings.CompactSidebar = request.CompactSidebar;

            settings.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return (
            true,
            "Settings updated successfully.",
            settings
        );
    }
}