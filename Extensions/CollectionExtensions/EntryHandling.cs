using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace aemarcoCommons.Extensions.CollectionExtensions
{
    public static class EntryHandling
    {
        private static readonly Random Random = new Random();
        public static List<T> Shuffle<T>(this IEnumerable<T> list)
        {
            var result = list.ToList();
            result = result.OrderBy(x => Random.Next()).ToList();
            return result;
        }



        [Obsolete("Use Chunk instead...")]
        public static List<List<T>> ChunkList<T>(this IEnumerable<T> list, int chunkSize)
        {
            if (chunkSize <= 0)
                throw new ArgumentException("chunkSize must be greater than 0.");


            var copy = list.ToList();

            var result = new List<List<T>>();
            while (copy.Count > 0)
            {
                var chunk = copy.Take(chunkSize).ToList();
                copy.RemoveRange(0, chunk.Count);
                result.Add(chunk);
            }
            return result;
        }

        public static IEnumerable<TSource[]> ChunkIterator<TSource>(IEnumerable<TSource> source, int size)
        {
            IEnumerator<TSource> e = source.GetEnumerator();

            // Before allocating anything, make sure there's at least one element.
            if (e.MoveNext())
            {
                // Now that we know we have at least one item, allocate an initial storage array. This is not
                // the array we'll yield.  It starts out small in order to avoid significantly overallocating
                // when the source has many fewer elements than the chunk size.
                int arraySize = Math.Min(size, 4);
                int i;
                do
                {
                    var array = new TSource[arraySize];

                    // Store the first item.
                    array[0] = e.Current;
                    i = 1;

                    if (size != array.Length)
                    {
                        // This is the first chunk. As we fill the array, grow it as needed.
                        for (; i < size && e.MoveNext(); i++)
                        {
                            if (i >= array.Length)
                            {
                                arraySize = (int)Math.Min((uint)size, 2 * (uint)array.Length);
                                Array.Resize(ref array, arraySize);
                            }

                            array[i] = e.Current;
                        }
                    }
                    else
                    {
                        // For all but the first chunk, the array will already be correctly sized.
                        // We can just store into it until either it's full or MoveNext returns false.
                        TSource[] local = array; // avoid bounds checks by using cached local (`array` is lifted to iterator object as a field)
                        Debug.Assert(local.Length == size);
                        for (; (uint)i < (uint)local.Length && e.MoveNext(); i++)
                        {
                            local[i] = e.Current;
                        }
                    }

                    if (i != array.Length)
                    {
                        Array.Resize(ref array, i);
                    }

                    yield return array;
                }
                while (i >= size && e.MoveNext());
            }
            e.Dispose();
        }

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
