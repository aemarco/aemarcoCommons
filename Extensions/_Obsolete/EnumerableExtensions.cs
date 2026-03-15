using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace aemarcoCommons.Extensions;

public static partial class EnumerableExtensions
{
    private static Random Random { get; } = new();

    [Obsolete("Use System.Linq instead")]
    public static List<T> Shuffle<T>(this IEnumerable<T> list)
    {
        var result = list.OrderBy(_ => Random.Next()).ToList();
        return result;
    }

}