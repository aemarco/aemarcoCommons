using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace Extensions.netExtensions
{
    public static class ListTExtension
    {
        private static readonly Random _random = new Random();

        [Obsolete("Use SortingExtensions instead")]
        public static List<T> Shuffle<T>(this List<T> list)
        {
            list = list.OrderBy(x => _random.Next()).ToList();
            return list;
        }
    }
}
