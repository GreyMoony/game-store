using AutoMapper;
using GameStore.Application.DTOs.CommentDtos;
using GameStore.Application.Helpers;
using GameStore.DAL.Interfaces;
using GameStore.Domain.Entities;
using GameStore.Domain.Entities.Mongo;
using GameStore.Domain.Exceptions;

namespace GameStore.Application.Services.CommentServices;
public class CommentService(
    IUnitOfWork unitOfWork,
    IUnitOfWorkMongo unitOfWorkMongo,
    IMapper mapper,
    EntityChangeLogger changeLogger) : ICommentService
{
    public const string DeletedCommentBody = "A comment/quote was deleted";

    public async Task<IEnumerable<CommentDto>> GetAllByGameKey(string gameKey)
    {
        var comments = await unitOfWork.Comments.GetAllByGameKeyAsync(gameKey);
        return mapper.Map<IEnumerable<CommentDto>>(comments);
    }

    public async Task AddCommentAsync(AddCommentDto comment)
    {
        var gameId = await GetGameIdByGameKey(comment.GameKey);

        string commentBody = comment.Body;
        if (comment.Action is CommentActions.Quote or CommentActions.Reply)
        {
            var parentComment = await unitOfWork.Comments.GetByIdAsync(comment.ParentId!.Value)
                ?? throw new EntityNotFoundException($"Parent comment with id {comment.ParentId} not found.");

            if (comment.Action == CommentActions.Reply)
            {
                // Format as a reply: "[Author], Text of Reply"
                commentBody = $"{parentComment.Name}, {commentBody}";
            }
            else if (comment.Action == CommentActions.Quote)
            {
                // Format as a quote: "[Body], Text of Quote"
                commentBody = $"{parentComment.Body}, {commentBody}";
            }
        }

        var commentEntity = new Comment()
        {
            Id = Guid.NewGuid(),
            Name = comment.Name,
            Body = commentBody,
            ParentCommentId = comment.ParentId,
            GameId = gameId,
        };

        await unitOfWork.Comments.AddAsync(commentEntity);
        await unitOfWork.SaveAsync();
    }

    public async Task DeleteCommentAsync(string key, Guid id, bool canManageDeletedGames)
    {
        await VerifyGame(key, canManageDeletedGames);
        var comment = await unitOfWork.Comments.GetByIdAsync(id)
            ?? throw new EntityNotFoundException($"Comment with id {id} for game with key {key} not found");

        UpdateChildComments(comment.Body, DeletedCommentBody, comment.ChildComments);
        comment.Body = DeletedCommentBody;

        unitOfWork.Comments.Update(comment);
        await unitOfWork.SaveAsync();
    }

    private static void UpdateChildComments(string oldParentBody, string newParentBody, IEnumerable<Comment> childComments)
    {
        if (childComments == null)
        {
            return;
        }

        foreach (var child in childComments)
        {
            if (!child.Body.StartsWith(oldParentBody, StringComparison.CurrentCulture))
            {
                continue;
            }

            var newBody = string.Concat(newParentBody, child.Body.AsSpan(oldParentBody.Length));
            UpdateChildComments(child.Body, newBody, child.ChildComments);
            child.Body = newBody;
        }
    }

    private async Task<Guid> GetGameIdByGameKey(string key)
    {
        var game = await unitOfWork.Games.GetByKeyAsync(key);
        if (game is null)
        {
            var product = await unitOfWorkMongo.Products.GetByKeyAsync(key)
                ?? throw new EntityNotFoundException($"Game with game key {key} not found");
            var gameId = await CopyProductToGameStoreDb(product);

            return gameId;
        }
        else
        {
            return game.Id;
        }
    }

    private async Task<Guid> CopyProductToGameStoreDb(Product product)
    {
        var game = mapper.Map<Game>(product);

        var genreId = await ProcessCategoryId(product.CategoryID!.Value);
        game.GameGenres =
            [
                new GameGenre
                {
                    GameId = game.Id,
                    GenreId = genreId,
                },
            ];

        var publisherId = await ProcessSupplierId(product.SupplierID!.Value);
        game.PublisherId = publisherId;

        await unitOfWork.Games.AddAsync(game);
        await unitOfWork.SaveAsync();

        await unitOfWorkMongo.Products.MarkAsCopiedAsync(product.ProductID!.Value);

        changeLogger.LogNorthwindEntityChange("COPY", product.GetType().Name, product, game);

        return game.Id;
    }

    private async Task<Guid> ProcessSupplierId(int supplierId)
    {
        var supplier = await unitOfWorkMongo.Suppliers.GetByIdAsync(supplierId);

        if (supplier.CopiedToSql.HasValue && supplier.CopiedToSql.Value)
        {
            var publisher = await unitOfWork.Publishers.GetBySupplierIdAsync(supplierId);
            return publisher.Id;
        }
        else
        {
            var publisher = mapper.Map<Publisher>(supplier);

            await unitOfWork.Publishers.AddAsync(publisher);
            await unitOfWork.SaveAsync();
            changeLogger.LogNorthwindEntityChange(
                "COPY", supplier.GetType().Name, supplier, publisher);
            await unitOfWorkMongo.Categories.MarkAsCopiedAsync(supplierId);

            return publisher.Id;
        }
    }

    private async Task<Guid> ProcessCategoryId(int categoryID)
    {
        var category = await unitOfWorkMongo.Categories.GetByIdAsync(categoryID);

        if (category.CopiedToSql.HasValue && category.CopiedToSql.Value)
        {
            var genre = await unitOfWork.Genres.GetByCategoryIdAsync(categoryID);
            return genre.Id;
        }
        else
        {
            var genre = mapper.Map<Genre>(category);

            await unitOfWork.Genres.AddAsync(genre);
            await unitOfWork.SaveAsync();
            changeLogger.LogNorthwindEntityChange(
                "COPY", category.GetType().Name, category, genre);
            await unitOfWorkMongo.Categories.MarkAsCopiedAsync(categoryID);

            return genre.Id;
        }
    }

    private async Task VerifyGame(string key, bool canManageDeletedGames)
    {
        var gameEntity = await unitOfWork.Games.GetByKeyAsync(key, canManageDeletedGames);

        if (!canManageDeletedGames && gameEntity is null)
        {
            var deletedGame = await unitOfWork.Games.GetByKeyAsync(key, true);
            if (deletedGame is null)
            {
                throw new EntityNotFoundException($"Game with key {key} not found.");
            }
            else
            {
                throw new UnauthenticatedException($"You have no rights to manage comments for game {key}");
            }
        }
        else if (canManageDeletedGames && gameEntity is null)
        {
            throw new EntityNotFoundException($"Game with key {key} not found.");
        }
    }
}
