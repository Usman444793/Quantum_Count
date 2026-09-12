using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quantum_Count.DTO.Staff;
using Quantum_Count.Services;
namespace Quantum_Count.Controllers;
[ApiController]
[Route("api/[controller]")]
//[Authorize(Roles = "Admin")]
public class StaffController : ControllerBase
{
    private readonly StaffService _staffService;
    public StaffController(StaffService staffService)
    {
        _staffService = staffService;
    }
    [HttpGet]
    public async Task<IActionResult> GetStaff()
    {
        var staff = await _staffService.GetStaffAsync();

        return Ok(staff);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetStaff(string id)
    {
        var staff = await _staffService.GetStaffAsync(id);
        if (staff == null)
            return NotFound(new
            {
                message = "Staff member not found."
            });
        return Ok(staff);
    }
    [HttpGet("roles")]
    public async Task<IActionResult> GetRoles()
    {
        var roles = await _staffService.GetRolesAsync();
        return Ok(roles);
    }
    [HttpPost]
    public async Task<IActionResult> CreateStaff([FromBody] CreateStaffRequest request)
    {
        var result = await _staffService.CreateStaffAsync(request);
        if (!result.Success)
        {
            return BadRequest(new
            {
                message = result.Message
            });
        }
        return Ok(result.Staff);
    }
    [HttpPut("{id}/role")]
    public async Task<IActionResult> UpdateRole(string id,[FromBody] UpdateStaffRoleRequest request)
    {
        var result = await _staffService.UpdateStaffRoleAsync(id,request);
        if (!result.Success)
        {
            return BadRequest(new
            {
                message = result.Message
            });
        }
        return Ok(new
        {
            message = result.Message
        });
    }
    [HttpPut("{id}/activate")]
    public async Task<IActionResult> ActivateStaff(string id)
    {
        var result = await _staffService.ActivateStaffAsync(id);
        if (!result.Success)
        {
            return NotFound(new
            {
                message = result.Message
            });
        }
        return Ok(new
        {
            message = result.Message
        });
    }
    [HttpPut("{id}/deactivate")]
    public async Task<IActionResult> DeactivateStaff( string id)
    {
        var result =await _staffService.DeactivateStaffAsync(id);
        if (!result.Success)
        {
            return NotFound(new
            {
                message = result.Message
            });
        }
        return Ok(new
        {
            message = result.Message
        });
    }
}