using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

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

        public static void AddDistinct<T, TComp>(this ICollection<T> collection, T item, Expression<Func<T, TComp>> selector)
        {
            Func<T, TComp> func = selector.Compile();
            if (collection.All(x => !func(x).Equals(func(item))))
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

        public static void AddRangeDistinct<T, TComp>(this ICollection<T> collection, IEnumerable<T> items, Expression<Func<T, TComp>> selector)
        {
            foreach (var item in items)
            {
                collection.AddDistinct(item, selector);
            }
        }


        /// <summary>
        /// Finds and returns the integer in the sequence that is closest to the given target integer.
        /// </summary>
        /// <param name="sequence">The sequence of integers to search for the closest integer.</param>
        /// <param name="target">The target integer to which the closest integer will be found.</param>
        /// <returns>The integer in the sequence that is closest to the target integer.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the input sequence is null.</exception>
        /// <exception cref="ArgumentException">Thrown if the input sequence is empty.</exception>
        public static int FindClosestItem(this IEnumerable<int> sequence, int target)
        {
            if (sequence == null)
                throw new ArgumentNullException(nameof(sequence));

            int? closest = null;
            var minDifference = int.MaxValue;
            foreach (var item in sequence)
            {
                var difference = Math.Abs(item - target);
                if (difference >= minDifference)
                    continue;

                closest = item;
                minDifference = difference;
            }

            return closest ?? throw new ArgumentException("Sequence is empty");
        }



    }
}
