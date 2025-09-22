namespace GameStore.Domain.Constants;
public static class UserRoles
{
    public const string Guest = "Guest";
    public const string User = "User";
    public const string Moderator = "Moderator";
    public const string Manager = "Manager";
    public const string Admin = "Admin";

    public static Dictionary<string, HashSet<string>> RolesPermissions => new()
    {
        {
            Guest,
            new HashSet<string> { Permissions.ReadOnlyAccess }
        },
        {
            User,
            new HashSet<string>
            {
                Permissions.ViewStockGames,
                Permissions.CommentGame,
            }
        },
        {
            Moderator,
            new HashSet<string>
            {
                Permissions.ManageGameComments,
                Permissions.BanUsersFromComments,
            }
        },
        {
            Manager,
            new HashSet<string>
            {
                Permissions.ManageEntities,
                Permissions.EditOrders,
                Permissions.ViewOrdersHistory,
            }
        },
        {
            Admin,
            new HashSet<string>
            {
                Permissions.ManageUsersAndRoles,
                Permissions.ViewDeletedGames,
                Permissions.ManageDeletedGameComments,
                Permissions.EditDeletedGames,
            }
        },
    };
}
