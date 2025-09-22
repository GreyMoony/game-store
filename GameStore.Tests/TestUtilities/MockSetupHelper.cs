using MongoDB.Driver;
using Moq;

namespace GameStore.Tests.TestUtilities;
public static class MockSetupHelper
{
    public static void SetupCursorMock<T>(
        Mock<IAsyncCursor<T>> cursorMock,
        IEnumerable<T> entities,
        bool hasMore)
        where T : class
    {
        cursorMock.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(hasMore))
            .Returns(Task.FromResult(false));

        cursorMock.Setup(cursor => cursor.Current)
            .Returns(entities);
    }
}
