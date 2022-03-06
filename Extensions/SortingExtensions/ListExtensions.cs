using System;
using System.Collections.Generic;
using System.Linq;

namespace aemarcoCommons.Extensions.SortingExtensions
{
    public static class ListExtensions
    {
        private static readonly Random Random = new Random();
        public static List<T> Shuffle<T>(this List<T> list)
        {
            list = list.OrderBy(x => Random.Next()).ToList();
            return list;
        }

        public static List<List<T>> Chunk<T>(this List<T> list, int chunkSize)
        {
            var copy = list.ToList();

            if (chunkSize <= 0)
            {
                throw new ArgumentException("chunkSize must be greater than 0.");
            }

            List<List<T>> result = new List<List<T>>();
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
