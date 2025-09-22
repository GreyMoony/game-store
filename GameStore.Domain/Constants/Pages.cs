namespace GameStore.Domain.Constants;
public static class Pages
{
    public const string Genres = "Genres";

    public const string Games = "Games";

    public const string Publishers = "Publishers";

    public const string Platforms = "Platforms";

    public const string Genre = "Genre";

    public const string UpdateGenre = "UpdateGenre";

    public const string DeleteGenre = "DeleteGenre";

    public const string AddGenre = "AddGenre";

    public const string Game = "Game";

    public const string UpdateGame = "UpdateGame";

    public const string DeleteGame = "DeleteGame";

    public const string AddGame = "AddGame";

    public const string Publisher = "Publisher";

    public const string UpdatePublisher = "UpdatePublisher";

    public const string DeletePublisher = "DeletePublisher";

    public const string AddPublisher = "AddPublisher";

    public const string Platform = "Platform";

    public const string UpdatePlatform = "UpdatePlatform";

    public const string DeletePlatform = "DeletePlatform";

    public const string AddPlatform = "AddPlatform";

    public const string History = "History";

    public const string Users = "Users";

    public const string User = "User";

    public const string UpdateUser = "UpdateUser";

    public const string DeleteUser = "DeleteUser";

    public const string AddUser = "AddUser";

    public const string Roles = "Roles";

    public const string Role = "Role";

    public const string UpdateRole = "UpdateRole";

    public const string DeleteRole = "DeleteRole";

    public const string AddRole = "AddRole";

    public const string Orders = "Orders";

    public const string Order = "Order";

    public const string UpdateOrder = "UpdateOrder";

    public const string ShipOrder = "ShipOrder";

    public const string Comments = "Comments";

    public const string ReplyComment = "ReplyComment";

    public const string QuoteComment = "QuoteComment";

    public const string DeleteComment = "DeleteComment";

    public const string BanComment = "BanComment";

    public const string Buy = "Buy";

    public const string Basket = "Basket";

    public const string MakeOrder = "MakeOrder";

    public const string AddComment = "AddComment";

    public const string Notifications = "Notifications";

    public static readonly HashSet<string> AllowedPages =
    [
        Genres, Genre, AddGenre, UpdateGenre, DeleteGenre,
        Games, Game, AddGame, UpdateGame, DeleteGame,
        Platforms, Platform, AddPlatform, UpdatePlatform, DeletePlatform,
        Publishers, Publisher, AddPublisher, UpdatePublisher, DeletePublisher,
        Users, User, UpdateUser, DeleteUser, AddUser,
        Roles, Role, UpdateRole, DeleteRole, AddRole,
        History, Orders, Order, UpdateOrder, ShipOrder,
        Comments, Buy, Basket, MakeOrder, AddComment,
        ReplyComment, QuoteComment, DeleteComment, BanComment,
        Notifications,
    ];

    public static readonly HashSet<string> AdminPages =
    [
        Users, User, AddUser, UpdateUser, DeleteUser,
        Roles, Role, AddRole, UpdateRole, DeleteRole
    ];

    public static readonly HashSet<string> ManagerPages =
    [
        History, Orders, Order, UpdateOrder,
        UpdateGame, UpdateGenre, UpdatePlatform, UpdatePublisher,
        AddGame, AddGenre, AddPlatform, AddPublisher,
        DeleteGame, DeleteGenre, DeletePlatform, DeletePublisher
    ];

    public static readonly HashSet<string> ManagerGamePages =
    [
        UpdateGame, AddGame, DeleteGame,
    ];

    public static readonly HashSet<string> ManagerGenrePages =
    [
        UpdateGenre, AddGenre, DeleteGenre,
    ];

    public static readonly HashSet<string> ManagerPlatformPages =
    [
        UpdatePlatform, AddPlatform, DeletePlatform,
    ];

    public static readonly HashSet<string> ManagerPublisherPages =
    [
        UpdatePublisher, AddPublisher, DeletePublisher,
    ];

    public static readonly HashSet<string> ManagerOrderPages =
    [
        Order, UpdateOrder, ShipOrder,
    ];

    public static readonly HashSet<string> ModeratorPages =
    [
        DeleteComment, BanComment,
    ];

    public static readonly HashSet<string> GuestPages =
    [
        Games, Genres, Publishers, Platforms
    ];

    public static readonly HashSet<string> UserPages =
    [
        Buy, AddComment, Basket, MakeOrder,
        Notifications
    ];

    public static readonly HashSet<string> UserCommentPages =
    [
        ReplyComment, QuoteComment
    ];
}
