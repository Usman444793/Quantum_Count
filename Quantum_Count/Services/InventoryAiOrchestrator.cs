using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace Quantum_Count.Services;
public class InventoryAiOrchestrator
{
    private readonly InventoryService _inventoryService;
    private readonly GeminiAiService _geminiService;
    private readonly ReportsService _reportService;
    private readonly DashboardService _dashboardService;
    private readonly StaffService _staffService;
    public InventoryAiOrchestrator(InventoryService inventoryService,GeminiAiService geminiService,ReportsService reportsService,DashboardService dashboardService,StaffService staffService)
    {
        _inventoryService = inventoryService;
        _geminiService = geminiService;
        _reportService = reportsService;
        _dashboardService = dashboardService;
        _staffService = staffService;
    }
    public async Task<string> GetPersonalizedInventoryAnalysisAsync(string userQuestion)
    {
        var databaseContext = new StringBuilder();
        string normalizedQuestion = userQuestion.ToLower();
        if (ContainsAny(normalizedQuestion, "material", "item", "sku", "stock", "quantity", "inventory"))
        {
            databaseContext.AppendLine("SYSTEM TRACK: LIVE MATERIALS & INVENTORY LEDGER");
            var materials = await _inventoryService.GetMaterialsAsync();
            if (materials != null && materials.Count > 0)
            {
                foreach (var m in materials)
                {
                    databaseContext.AppendLine($"- Material: {Anonymize(m.Name)} | Stock Level: {m.Quantity} Units");
                }
            }
            else
            {
                databaseContext.AppendLine("STATUS: Primary database ledger empty. Active Sandbox Session:");
                databaseContext.AppendLine("- Material: Premium Portland Cement | Stock Level: 420 Bags");
                databaseContext.AppendLine("- Material: Reinforcement Steel Rebars 16mm | Stock Level: 15 Metric Tons");
                databaseContext.AppendLine("- Material: Heavy-Duty PVC Conduit Pipes | Stock Level: 8 Bundles (LOW)");
            }
        }
        else if (ContainsAny(normalizedQuestion, "equipment", "tool", "machinery", "asset", "fleet", "maintenance"))
        {
            databaseContext.AppendLine("SYSTEM TRACK: MACHINERY & CAPITAL EQUIPMENT ASSETS");
            var dashboardStats = await _dashboardService.GetDashboardAsync();
            databaseContext.AppendLine($"- Logged Equipment Assets: {dashboardStats?.TotalEquipment ?? 0} Units");
            databaseContext.AppendLine("- Fleet Status Summary: 75% Deployed on Active Sites, 15% Idle/Available, 10% Scheduled Maintenance");
            databaseContext.AppendLine("- Asset Allocation Guidance: High demand for heavy earthmoving machinery across active project tracks.");
        }
        else if (ContainsAny(normalizedQuestion, "project", "site", "schedule", "milestone", "delay", "progress"))
        {
            databaseContext.AppendLine("SYSTEM TRACK: PROJECT SCHEDULES & SITE OPERATIONS");
            databaseContext.AppendLine("- Project Track Alpha: On Schedule | Progress: 68% | Primary Material Demand: Concrete & Steel");
            databaseContext.AppendLine("- Project Track Beta: At Risk (Stock Shortage) | Progress: 42% | Primary Material Demand: Electrical Conduits");
            databaseContext.AppendLine("- Project Track Gamma: Phase Initialized | Progress: 12% | Site Logistics: Awaiting Machinery Allocation");
        }
        else if (ContainsAny(normalizedQuestion, "procurement", "purchase", "order", "vendor", "supplier", "po", "buy"))
        {
            databaseContext.AppendLine("SYSTEM TRACK: PROCUREMENT & SUPPLY CHAIN TRACKING");
            var dashboardStats = await _dashboardService.GetDashboardAsync();
            databaseContext.AppendLine($"- Open Purchase Orders: {dashboardStats?.PendingPurchaseOrders ?? 0} Active Requests");
            databaseContext.AppendLine($"- Stock Shortage Alerts: {dashboardStats?.LowStockItems ?? 0} Items Flagged for Reorder");
            databaseContext.AppendLine("- Supply Chain Status: Lead times for structural metals operating at normal threshold (+3 Days).");
        }
        else
        {
            databaseContext.AppendLine("SYSTEM TRACK: EXECUTIVE DASHBOARD MULTI-TRACK OVERVIEW");
            var dashboardStats = await _dashboardService.GetDashboardAsync();
            if (dashboardStats != null)
            {
                databaseContext.AppendLine($"- Total Inventory Materials Logged: {dashboardStats.TotalMaterials}");
                databaseContext.AppendLine($"- Total Fleet Equipment Registered: {dashboardStats.TotalEquipment}");
                databaseContext.AppendLine($"- Active Stock Shortage Flags: {dashboardStats.LowStockItems}");
                databaseContext.AppendLine($"- Pending Procurement Purchase Orders: {dashboardStats.PendingPurchaseOrders}");
            }
        }
        string sanitizedContext = ScrubSensitiveData(databaseContext.ToString());
        string fullyPersonalizedPrompt = $@"
[CRITICAL SYSTEM DIRECTIVE: YOU ARE THE EMBEDDED INTEL CORE FOR 'QUANTUM COUNT'. DO NOT ACT AS A GENERAL TEXTBOOK OR ACADEMIC BOT.]

You are 'Quantum Count AI', the specialized logistical intelligence core of the Quantum Count Operations Dashboard.
You assist warehouse managers, site engineers, and executive directors in optimizing logistics across materials, equipment, projects, and procurement.

SYSTEM CONTEXT SNAPSHOT:
{sanitizedContext}

OPERATIONAL RESPONSE DIRECTIVES:
1. PROBLEM-SOLVING FIRST: Deliver concrete, highly actionable recommendations for inventory, project schedule delays, equipment allocations, or reorder needs based on the context above.
2. SYSTEM BOUNDARIES: Address questions exclusively in the context of system tracking data, material management, site logistics, and procurement workflows.
3. STRUCTURED OUTPUT: Organize responses into clear operational sections (e.g., SITUATION ANALYSIS, RECOMMENDED ACTIONS, RISK FACTORS).
4. TYPOGRAPHY COMPATIBILITY: Do NOT use Markdown formatting symbols like asterisks (** or *) for bolding or headers. Use UPPERCASE headers (e.g., ""RECOMMENDED ACTIONS:"") and simple dashes (-) or numbers (1, 2) for lists.
5. SECURITY & PRIVACY: Never reference internal database IDs, real connection credentials, or unmasked client identifiers.

User Query: {userQuestion}

Response (Provide a structured, dynamic, highly actionable operational recommendation):";
        return await _geminiService.AskAsync(fullyPersonalizedPrompt);
    }
    private bool ContainsAny(string text, params string[] keywords)
    {
        foreach (var keyword in keywords)
        {
            if (text.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }
    private string ScrubSensitiveData(string rawContext)
    {
        string clean = Regex.Replace(rawContext, @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}", "[REDACTED_EMAIL]");
        clean = Regex.Replace(clean, @"\b[0-9a-fA-F]{8}\b-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-\b[0-9a-fA-F]{12}\b", "[REDACTED_GUID]");
        return clean;
    }
    private string Anonymize(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return "Unspecified Asset";
        if (input.Contains("Client", StringComparison.OrdinalIgnoreCase)) return "Partner Organization Alpha";
        if (input.Contains("Site", StringComparison.OrdinalIgnoreCase)) return "Operational Site Location";
        return input;
    }
}