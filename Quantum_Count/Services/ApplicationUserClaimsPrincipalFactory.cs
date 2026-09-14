using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Quantum_Count.Models;
namespace Quantum_Count.Services;
public class ApplicationUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUsers, IdentityRole>
{
    public ApplicationUserClaimsPrincipalFactory(UserManager<ApplicationUsers> userManager,RoleManager<IdentityRole> roleManager,
        IOptions<IdentityOptions> optionsAccessor) : base(userManager,roleManager,optionsAccessor)
    {
    }
    protected override async Task<ClaimsIdentity>
        GenerateClaimsAsync(ApplicationUsers user)
    {
        var identity = await base.GenerateClaimsAsync(user);
        identity.AddClaim(new Claim("FullName",user.FullName ?? string.Empty));
        return identity;
    }
}