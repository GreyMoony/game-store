using GameStore.DAL.Data;
using GameStore.DAL.Interfaces;

namespace GameStore.DAL.Repositories;

#pragma warning disable S3604
public class UnitOfWork(GameStoreContext context,
    IGameRepository games,
    IGenreRepository genres,
    IPlatformRepository platforms,
    IPublisherRepository publishers,
    ICommentRepository comments,
    IOrderRepository orders) : IUnitOfWork
{
    private bool _disposed;

    public IGameRepository Games { get; } = games;

    public IGenreRepository Genres { get; } = genres;

    public IPlatformRepository Platforms { get; } = platforms;

    public IPublisherRepository Publishers { get; } = publishers;

    public IOrderRepository Orders { get; } = orders;

    public ICommentRepository Comments { get; } = comments;

    public async Task<int> SaveAsync()
    {
        return await context.SaveChangesAsync();
    }

    public void Dispose()
    {
        Dispose(true);

        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                context.Dispose();
            }

            _disposed = true;
        }
    }
}
