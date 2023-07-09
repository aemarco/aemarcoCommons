using System;
using System.Collections.Generic;
using System.Linq;

namespace aemarcoCommons.Extensions
{
    public static class EnumerableExtensions
    {
        internal static Random Random { get; set; } = new Random();
        public static List<T> Shuffle<T>(this IEnumerable<T> list)
        {
            var result = list.OrderBy(x => Random.Next()).ToList();
            return result;
        }

        /// <summary>
        /// Checks is not null and contains elements
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="source">collection to check</param>
        /// <returns>true when collection with elements</returns>
        public static bool NotNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return source != null && source.Any();
        }

        /// <summary>
        /// Checks is null or contains no elements
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="source">collection to check</param>
        /// <returns>true when collection is null or empty</returns>
        public static bool NullOrEmpty<T>(this IEnumerable<T> source) => !source.NotNullOrEmpty();



        public static IEnumerable<(int Min, int Max)> ConsolidateRanges(this IEnumerable<(int Min, int Max)> ranges)
        {
            (int Min, int Max)? current = null;
            foreach (var range in ranges.OrderBy(x => x.Min))
            {
                if (current == null)
                {
                    current = range;
                    continue;
                }

                if (current.Value.Max >= range.Min - 1)
                {
                    current = (current.Value.Min, Math.Max(current.Value.Max, range.Max));
                }
                else
                {
                    yield return current.Value;
                    current = range;
                }
            }

            if (current != null)
            {
                yield return current.Value;
            }

            //var result = new List<(int Min, int Max)>();
            //foreach (var range in ranges.OrderBy(x => x.Min).ToList())
            //{
            //    if (result.NotNullOrEmpty())
            //    {
            //        var last = result.Last();
            //        if (last.Max >= range.Min - 1)
            //        {
            //            result.Remove(last);
            //            result.Add((last.Min, Math.Max(last.Max, range.Max)));
            //            continue;
            //        }
            //    }
            //    result.Add(range);
            //}
            //return result;
        }

        public static void AddDistinct<T>(this ICollection<T> collection, T item)
        {
            if (!collection.Contains(item))
            {
                collection.Add(item);
            }
        }
        public static void AddRangeDistinct<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                collection.AddDistinct(item);
            }
        }
    }
}
