using Moq;

namespace GameStore.Tests.TestUtilities.MockQuery;
public static class AsyncMockExtensions
{
    public static Mock<IQueryable<T>> BuildMock<T>(this IQueryable<T> data)
        where T : class
    {
        var mock = new Mock<IQueryable<T>>();
        mock.As<IAsyncEnumerable<T>>()
            .Setup(x => x.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new TestAsyncEnumerator<T>(data.GetEnumerator()));
        mock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<T>(data.Provider));
        mock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
        mock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
        mock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

        return mock;
    }
}