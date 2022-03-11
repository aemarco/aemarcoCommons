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
    }


    
}
