using GameStore.DAL.Data;
using GameStore.DAL.Interfaces;
using GameStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.DAL.Repositories;
#pragma warning disable IDE0301 // Simplify collection initialization
public class CommentRepository(GameStoreContext context)
    : Repository<Comment>(context), ICommentRepository
{
    public async Task<IEnumerable<Comment>> GetAllByGameKeyAsync(string gameKey)
    {
        var game = await Context.Games.Include(g => g.Comments)
            .ThenInclude(c => c.ChildComments)
            .FirstOrDefaultAsync(g => g.Key == gameKey);
        return game is null ?
            Enumerable.Empty<Comment>()
            : game.Comments.Where(c => c.ParentCommentId is null);
    }

    public async Task<Comment?> GetByIdAsync(Guid id)
    {
        var comment = await Context.Comments.Include(c => c.ChildComments)
            .FirstOrDefaultAsync(c => c.Id == id);
        return comment;
    }
}
#pragma warning restore IDE0301 // Simplify collection initialization
