using GameStore.DAL.Data;
using GameStore.DAL.SeedData;
using GameStore.Tests.TestUtilities;

namespace GameStore.Tests.RepositoriesTests;
public class GameStoreSeedDataTests : IDisposable
{
    private readonly GameStoreContext _context;
    private bool _disposed;

    public GameStoreSeedDataTests()
    {
        _context = GameStoreContextFactory.CreateInMemoryContext();
    }

    [Fact]
    public void Seed_ShouldSeedDbContext()
    {
        // Act
        GameStoreSeedData.Seed(_context, 10);

        // Assert
        Assert.NotEmpty(_context.Games.ToList());
    }

    public void Dispose()
    {
        // Call the Dispose method with disposing set to true
        Dispose(true);

        // Suppress finalization
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Dispose managed resources
                _context.Dispose();
            }

            // Set the disposed flag to true
            _disposed = true;
        }
    }
}
