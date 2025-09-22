namespace GameStore.DAL.Interfaces;
public interface IUnitOfWork : IDisposable
{
    IGameRepository Games { get; }

    IGenreRepository Genres { get; }

    IPlatformRepository Platforms { get; }

    IPublisherRepository Publishers { get; }

    IOrderRepository Orders { get; }

    ICommentRepository Comments { get; }

    Task<int> SaveAsync();
}
