using GameStore.Application.DTOs.CommentDtos;

namespace GameStore.Application.Services.CommentServices;
public interface ICommentService
{
    Task<IEnumerable<CommentDto>> GetAllByGameKey(string gameKey);

    Task AddCommentAsync(AddCommentDto comment);

    Task DeleteCommentAsync(string key, Guid id, bool canManageDeletedGames);
}
