﻿using System.Linq.Expressions;

namespace GameStore.Tests.TestUtilities.MockQuery;

internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>
{
    public TestAsyncEnumerable(IEnumerable<T> enumerable)
        : base(enumerable)
    {
    }

    public TestAsyncEnumerable(Expression expression)
        : base(expression)
    {
    }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
        new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
}
