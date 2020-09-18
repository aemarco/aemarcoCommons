using System;
using System.Collections.Generic;
using System.Linq;

namespace aemarcoCommons.Extensions.SortingExtensions
{
    public static class ListExtensions
    {
        private static readonly Random _random = new Random();
        public static List<T> Shuffle<T>(this List<T> list)
        {
            list = list.OrderBy(x => _random.Next()).ToList();
            return list;
        }
    }
}
