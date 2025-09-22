using System.Security.Claims;
using GameStore.Domain.Constants;
using GameStore.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace GameStore.DAL.SeedData;
public static class IdentitySeedData
{
    public static async Task SeedIdentityDataAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Implement role inheritance
        var inheritedPermissions = GetInheritedPermissions(UserRoles.RolesPermissions);
        await CreateRolesAndAssignClaims(roleManager, inheritedPermissions);
        await CreateAdminUser(userManager);
    }

    private static Dictionary<string, HashSet<string>> GetInheritedPermissions(Dictionary<string, HashSet<string>> roleHierarchy)
    {
        var inheritedPermissions = new Dictionary<string, HashSet<string>>();

        foreach (var role in roleHierarchy.Keys)
        {
            inheritedPermissions[role] =
                [.. roleHierarchy[role]];

            if (role != "Guest")
            {
                var lowerRoleIndex = roleHierarchy.Keys.ToList().IndexOf(role) - 1;
                if (lowerRoleIndex >= 0)
                {
                    var lowerRole = roleHierarchy.Keys.ElementAt(lowerRoleIndex);
                    inheritedPermissions[role].UnionWith(inheritedPermissions[lowerRole]);
                }
            }
        }

        return inheritedPermissions;
    }

    private static async Task CreateRolesAndAssignClaims(RoleManager<IdentityRole> roleManager, Dictionary<string, HashSet<string>> inheritedPermissions)
    {
        foreach (var role in inheritedPermissions)
        {
            var roleExists = await roleManager.FindByNameAsync(role.Key);
            if (roleExists == null)
            {
                await roleManager.CreateAsync(new IdentityRole(role.Key));
                roleExists = await roleManager.FindByNameAsync(role.Key);
            }

            var existingClaims = await roleManager.GetClaimsAsync(roleExists!);
            foreach (var permission in role.Value)
            {
                if (!existingClaims.Any(c => c.Type == "Permission" && c.Value == permission))
                {
                    await roleManager.AddClaimAsync(roleExists!, new Claim("Permission", permission));
                }
            }
        }
    }

    private static async Task CreateAdminUser(UserManager<ApplicationUser> userManager)
    {
        var adminUsername = "admin";
        var adminPassword = "Admin@123";

        var adminUser = await userManager.FindByNameAsync(adminUsername);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser { UserName = adminUsername };
            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException("Failed to create admin user.");
            }
        }

        if (!await userManager.IsInRoleAsync(adminUser, UserRoles.Admin))
        {
            await userManager.AddToRoleAsync(adminUser, UserRoles.Admin);
        }
    }
}
