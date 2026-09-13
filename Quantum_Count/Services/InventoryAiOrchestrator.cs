using System;
using System.Text;
using System.Threading.Tasks;

namespace Quantum_Count.Services;

public class InventoryAiOrchestrator
{
    private readonly InventoryService _inventoryService;
    private readonly GeminiAiService _geminiService;
    private readonly ReportsService _reportService;
    private readonly DashboardService _dashboardService;

    public InventoryAiOrchestrator(
        InventoryService inventoryService,
        GeminiAiService geminiService,
        ReportsService reportsService,
        DashboardService dashboardService)
    {
        _inventoryService = inventoryService;
        _geminiService = geminiService;
        _reportService = reportsService;
        _dashboardService = dashboardService;
    }

    public async Task<string> GetPersonalizedInventoryAnalysisAsync(string userQuestion)
    {
        var databaseContext = new StringBuilder();
        string normalizedQuestion = userQuestion.ToLower();

        if (normalizedQuestion.Contains("material") || normalizedQuestion.Contains("item") || normalizedQuestion.Contains("sku"))
        {
            databaseContext.AppendLine("Current System Database Context (Live Stock & Material Ledger):");

            // Your exact method without any threshold parameters
            var materials = await _inventoryService.GetMaterialsAsync();

            if (materials != null && materials.Count > 0)
            {
                foreach (var m in materials)
                {
                    databaseContext.AppendLine($"- Material: {m.Name} | Qty In Stock: {m.Quantity}");
                }
            }
            else
            {
                // FIX: Instead of a generic string, inject warehouse mock context here.
                // This guarantees the AI reads "Cement" or "Steel" and stays in character.
                databaseContext.AppendLine("⚠️ SYSTEM ALERT: Active Database Ledger is currently empty of assets.");
                databaseContext.AppendLine("Displaying active system sandbox inventory data for current session evaluation:");
                databaseContext.AppendLine("- Material: Premium Portland Cement | Qty In Stock: 420 Bags");
                databaseContext.AppendLine("- Material: Reinforcement Steel Rebars 16mm | Qty In Stock: 15 Metric Tons");
                databaseContext.AppendLine("- Material: Heavy-Duty PVC Conduit Pipes | Qty In Stock: 8 Bundles (CRITICAL LOW)");
            }
        }
        else if (normalizedQuestion.Contains("equipment") || normalizedQuestion.Contains("tool") || normalizedQuestion.Contains("asset"))
        {
            databaseContext.AppendLine("Current System Database Context (Machinery & Asset Allocations):");
            var dashboardStats = await _dashboardService.GetDashboardAsync();
            databaseContext.AppendLine($"- Total Equipment Assets Logged: {dashboardStats?.TotalEquipment ?? 0}");
            databaseContext.AppendLine("- Note: Detailed equipment assignment logs are stored securely in the tracking sub-ledger.");
        }
        else
        {
            databaseContext.AppendLine("Current System Database Context (Executive Dashboard Summary Snapshot):");
            var dashboardStats = await _dashboardService.GetDashboardAsync();
            if (dashboardStats != null)
            {
                databaseContext.AppendLine($"- Total Tracked Materials: {dashboardStats.TotalMaterials}");
                databaseContext.AppendLine($"- Total Capital Equipment: {dashboardStats.TotalEquipment}");
                databaseContext.AppendLine($"- Active Stock Shortage Flags: {dashboardStats.LowStockItems}");
                databaseContext.AppendLine($"- Open Procurement Purchase Orders: {dashboardStats.PendingPurchaseOrders}");
            }
        }

        string fullyPersonalizedPrompt = $@"
[CRITICAL SYSTEM DIRECTIVE: YOU ARE THE EMBEDDED AI FOR THE 'QUANTUM COUNT' MANAGEMENT DASHBOARD. NEVER TALK ABOUT THE SCIENCE OF METALS, POLYMERS, OR CERAMICS. ONLY TALK ABOUT SYSTEM ITEMS.]

You are 'Quantum Count AI', the specialized, embedded intelligence core of the Quantum Count Inventory Management System. 
You are NOT an academic textbook assistant or a general knowledge model. You are a conversational software feature helping a warehouse manager.

CRITICAL INSTRUCTIONS:
1. Speak directly to the system operator as an expert technical logistics adviser.
2. Read the live database context attached below. Use it as your SOLE source of truth to answer questions about 'materials'.
3. If the user asks about materials, analyze the material names, quantities, and operational stock levels from the text stream below. Do not discuss raw physics or chemical properties unless it directly addresses a recorded stock item.
4. If the database context shows placeholder sandbox materials, mention to the manager that they should use the system dashboard interface forms to begin inputting real records.
5. CRITICAL TYPOGRAPHY RULE: Do not use Markdown formatting symbols like asterisks (** or *) for bolding or list formatting. 
6. Instead, use regular ALL-CAPS text for headers (e.g., ""ALERT:"") and simple dashes (-) or numbers (1, 2) on new lines for lists. Keep it clean and compatible with plain text display.
---
{databaseContext}
---

User Query: {userQuestion}

Response (Provide a clean, professional, highly actionable response customized to our system tracking data):";

        // 3. Send the structured context straight to your Gemini Service client wrapper
        return await _geminiService.AskAsync(fullyPersonalizedPrompt);
    }
}
