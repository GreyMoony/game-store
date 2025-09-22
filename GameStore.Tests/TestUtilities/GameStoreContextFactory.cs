using GameStore.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Tests.TestUtilities;
public static class GameStoreContextFactory
{
    public static GameStoreContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<GameStoreContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new GameStoreContext(options);
    }

    public static AuthDbContext CreateInMemoryAuthContext()
    {
        var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AuthDbContext(options);
    }
}
