namespace GameStore.Tests.TestUtilities.MockQuery;

internal class TestAsyncEnumerator<T>(IEnumerator<T> inner) : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> _inner = inner;

    public T Current => _inner.Current;

    public ValueTask DisposeAsync()
    {
        _inner.Dispose();
        return default;
    }

    public ValueTask<bool> MoveNextAsync() => new(_inner.MoveNext());
}