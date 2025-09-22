using GameStore.Domain.Constants;

namespace GameStore.API.Extensions;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
        .AddPolicy("CanCommentGame", policy =>
            policy.RequireClaim(Permissions.PermissionClaim, Permissions.CommentGame))
        .AddPolicy("CanViewGamesInStock", policy =>
            policy.RequireClaim(Permissions.PermissionClaim, Permissions.ViewStockGames))
        .AddPolicy("CanManageComments", policy =>
            policy.RequireClaim(Permissions.PermissionClaim, Permissions.ManageGameComments))
        .AddPolicy("CanBanUsers", policy =>
            policy.RequireClaim(Permissions.PermissionClaim, Permissions.BanUsersFromComments))
        .AddPolicy("CanManageUsersAndRoles", policy =>
            policy.RequireClaim(Permissions.PermissionClaim, Permissions.ManageUsersAndRoles))
        .AddPolicy("CanManageEntities", policy =>
            policy.RequireClaim(Permissions.PermissionClaim, Permissions.ManageEntities))
        .AddPolicy("CanEditOrders", policy =>
            policy.RequireClaim(Permissions.PermissionClaim, Permissions.EditOrders))
        .AddPolicy("CanViewOrdersHistory", policy =>
            policy.RequireClaim(Permissions.PermissionClaim, Permissions.ViewOrdersHistory));

        return services;
    }
}
