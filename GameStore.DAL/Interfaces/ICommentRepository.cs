using GameStore.Domain.Entities;

namespace GameStore.DAL.Interfaces;
public interface ICommentRepository : IRepository<Comment>
{
    Task<IEnumerable<Comment>> GetAllByGameKeyAsync(string gameKey);
}
