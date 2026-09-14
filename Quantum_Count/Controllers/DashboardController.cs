using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Quantum_Count.Services;
namespace Quantum_Count.Controllers;
[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly DashboardService _dashboardService;
    public DashboardController(DashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }
    [HttpGet]
    public async Task<IActionResult> GetDashboardAsync()
    {
        var dashboard = await _dashboardService.GetDashboardAsync();
        return Ok(dashboard);
    }
}