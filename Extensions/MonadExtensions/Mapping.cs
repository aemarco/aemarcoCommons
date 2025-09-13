using System;

namespace aemarcoCommons.Extensions.MonadExtensions;

public static class Mapping
{
    public static TOut Map<TIn, TOut>(this TIn @this, Func<TIn, TOut> f) =>
        f(@this);
}