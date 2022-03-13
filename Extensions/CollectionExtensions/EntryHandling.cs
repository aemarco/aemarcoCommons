using System;
using System.Collections.Generic;
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

        public static List<List<T>> Chunk<T>(this IEnumerable<T> list, int chunkSize)
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

        public static IEnumerable<(int Min, int Max)> Consolidate(this IEnumerable<(int Min, int Max)> ranges)
        {
            var result = new List<(int Min, int Max)>();
            foreach (var range in ranges.OrderBy(x => x.Min).ToList())
            {
                if (result.NotNullOrEmpty())
                {
                    var last = result.Last();
                    if (last.Max >= range.Min - 1)
                    {
                        result.Remove(last);
                        result.Add((last.Min, Math.Max(last.Max, range.Max)));
                        continue;
                    }
                }
                result.Add(range);
            }
            return result;
        }








    }


    
}
