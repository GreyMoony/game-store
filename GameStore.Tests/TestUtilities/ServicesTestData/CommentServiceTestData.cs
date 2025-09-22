using GameStore.Application.DTOs.CommentDtos;
using GameStore.Application.Services.CommentServices;
using GameStore.Domain.Entities;
using GameStore.Domain.Entities.Mongo;

namespace GameStore.Tests.TestUtilities.ServicesTestData;
public static class CommentServiceTestData
{
    public static List<Comment> CommentsList =>
    [
            new() { Id = Guid.NewGuid(), Name = "User 1", Body = "Comment 1" },
            new() { Id = Guid.NewGuid(), Name = "User 2", Body = "Comment 1" },
    ];

    public static Game Game => new()
    {
        Id = Guid.NewGuid(),
        Key = "gameKey",
    };

    public static Product Product => new()
    {
        Key = "gameKey",
        CategoryID = 1,
        SupplierID = 1,
        ProductID = 1,
    };

    public static Category Category => new()
    {
        CategoryID = 1,
    };

    public static Supplier Supplier => new()
    {
        SupplierID = 1,
    };

    public static Comment ParentComment => new()
    {
        Id = Guid.NewGuid(),
        Body = "Parent comment",
        Name = "Author",
        GameId = Game.Id,
    };

    public static IEnumerable<object[]> AddCommentTestData =>
    [
        [
            new AddCommentDto
            {
                GameKey = Game.Key,
                Name = "Test user",
                Body = "Reply text",
                ParentId = ParentComment.Id,
                Action = CommentActions.Reply,
            },
            $"{ParentComment.Name}, Reply text"
        ],
        [
            new AddCommentDto
            {
                GameKey = Game.Key,
                Name = "Test user",
                Body = "Quote text",
                ParentId = ParentComment.Id,
                Action = CommentActions.Quote,
            },
            $"{ParentComment.Body}, Quote text"
        ]
    ];
}
