using System.Collections.Generic;
using System.Linq;

namespace aemarcoCommons.Extensions;

public static class DictionaryExtensions
{
    public static IDictionary<TK, TV> ToDictionary<TK, TV>(this IEnumerable<(TK, TV)> @this) =>
        @this.ToDictionary(
            x => x.Item1,
            x => x.Item2);

    public static IDictionary<TK, TV> ToDictionary<TK, TV>(this IEnumerable<KeyValuePair<TK, TV>> @this) =>
        @this.ToDictionary(
            x => x.Key,
            x => x.Value);

    public static IDictionary<TK, ICollection<TV>> ToDictionary<TK, TV>(this IEnumerable<IGrouping<TK, TV>> @this) =>
        @this.ToDictionary(
            x => x.Key,
            x => (ICollection<TV>)x.ToList());


    public static IDictionary<TK, int> ToCountDictionary<TK, TV>(this IEnumerable<IGrouping<TK, TV>> @this) =>
        @this.ToDictionary(
            x => x.Key,
            x => x.Count());

    public static IDictionary<TK, int> ToCountDictionary<TK>(this IEnumerable<TK> @this) =>
        @this.GroupBy(x => x)
            .ToCountDictionary();
}