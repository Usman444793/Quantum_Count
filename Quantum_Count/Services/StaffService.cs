using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Quantum_Count.DTO.Staff;
using Quantum_Count.Models;
namespace Quantum_Count.Services;
public class StaffService
{
    private readonly UserManager<ApplicationUsers> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public StaffService(UserManager<ApplicationUsers> userManager,RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }
    public async Task<List<StaffDto>> GetStaffAsync()
    {
        var users = await _userManager.Users.OrderBy(u => u.FullName).ToListAsync();
        var result = new List<StaffDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            result.Add(new StaffDto
            {
                Id = user.Id,
                FullName = user.FullName ?? "",
                Email = user.Email ?? "",
                CreatedAt = user.CreatedAt,
                IsActive = !await _userManager.IsLockedOutAsync(user),
                Roles = roles.ToList()
            });
        }
        return result;
    }
    public async Task<StaffDto?> GetStaffAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return null;
        var roles = await _userManager.GetRolesAsync(user);
        return new StaffDto
        {
            Id = user.Id,
            FullName = user.FullName ?? "",
            Email = user.Email ?? "",
            CreatedAt = user.CreatedAt,
            IsActive = !await _userManager.IsLockedOutAsync(user),
            Roles = roles.ToList()
        };
    }
    public async Task<(bool Success, string Message, StaffDto? Staff)> CreateStaffAsync(CreateStaffRequest request)
    {
        if (!await _roleManager.RoleExistsAsync(request.Role))
        {
            return (false, "Selected role does not exist.", null);
        }
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return (false, "A staff member with this email already exists.", null);
        }
        var user = new ApplicationUsers
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            CreatedAt = DateTime.UtcNow,
            EmailConfirmed = true,
            LockoutEnabled = true
        };
        var createResult = await _userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            var errors = string.Join( ", ",createResult.Errors.Select(e => e.Description));
            return (false, errors, null);
        }
        var roleResult =await _userManager.AddToRoleAsync(user,request.Role);
        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);
            var errors = string.Join(", ",roleResult.Errors.Select(e => e.Description));
            return (false, errors, null);
        }
        return (true,"Staff member created successfully.",await GetStaffAsync(user.Id)
        );
    }
    public async Task<(bool Success, string Message)>
        UpdateStaffRoleAsync(string id,UpdateStaffRoleRequest request)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return (false, "Staff member not found.");
        if (!await _roleManager.RoleExistsAsync(request.Role))
            return (false, "Selected role does not exist.");
        var currentRoles =await _userManager.GetRolesAsync(user);
        if (currentRoles.Any())
        {
            var removeResult = await _userManager.RemoveFromRolesAsync(user,currentRoles);
            if (!removeResult.Succeeded)
            {
                return (false,string.Join(", ",removeResult.Errors.Select(e => e.Description)));
            }
        }
        var addResult = await _userManager.AddToRoleAsync( user, request.Role);
        if (!addResult.Succeeded)
        {
            return (false,string.Join( ", ",addResult.Errors.Select(e => e.Description)));
        }
        return (true, "Staff role updated successfully.");
    }
    public async Task<(bool Success, string Message)>
        ActivateStaffAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return (false, "Staff member not found.");
        await _userManager.SetLockoutEndDateAsync( user,null);
        return (true, "Staff member activated successfully.");
    }
    public async Task<(bool Success, string Message)>DeactivateStaffAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return (false, "Staff member not found.");
        await _userManager.SetLockoutEnabledAsync(user,true);
        await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
        return (true, "Staff member deactivated successfully.");
    }
    public async Task<List<string>> GetRolesAsync()
    {
        return await _roleManager.Roles.OrderBy(r => r.Name).Select(r => r.Name!).ToListAsync();
    }
}