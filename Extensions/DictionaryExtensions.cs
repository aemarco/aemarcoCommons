using System.Collections.Generic;
using System.Linq;

namespace aemarcoCommons.Extensions;

public static class DictionaryExtensions
{
    public static IDictionary<Tk, Tv> ToDictionary<Tk, Tv>(this IEnumerable<(Tk, Tv)> @this) =>
        @this.ToDictionary(
            x => x.Item1,
            x => x.Item2);

    public static IDictionary<Tk, Tv> ToDictionary<Tk, Tv>(this IEnumerable<KeyValuePair<Tk, Tv>> @this) =>
        @this.ToDictionary(
            x => x.Key,
            x => x.Value);

    public static IDictionary<Tk, ICollection<Tv>> ToDictionary<Tk, Tv>(this IEnumerable<IGrouping<Tk, Tv>> @this) =>
        @this.ToDictionary(
            x => x.Key,
            x => (ICollection<Tv>)x.ToList());


    public static IDictionary<Tk, int> ToCountDictionary<Tk, Tv>(this IEnumerable<IGrouping<Tk, Tv>> @this) =>
        @this.ToDictionary(
            x => x.Key,
            x => x.Count());

    public static IDictionary<Tk, int> ToCountDictionary<Tk>(this IEnumerable<Tk> @this) =>
        @this.GroupBy(x => x)
            .ToCountDictionary();
}