using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quantum_Count.Models;
using Quantum_Count.Services;

namespace Quantum_Count.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class SettingsController : ControllerBase
{
    private readonly SettingsService _settingsService;

    public SettingsController(SettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetSettings()
    {
        var settings = await _settingsService.GetSettingsAsync();

        return Ok(settings);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateSettings(
        [FromBody] ApplicationSettings settings)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _settingsService.UpdateSettingsAsync(settings);

        if (!result.success)
            return BadRequest(new
            {
                message = result.message
            });

        return Ok(new
        {
            message = result.message,
            settings = result.settings
        });
    }
}