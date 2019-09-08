using System;
using System.Collections.Generic;
using System.Linq;

namespace Extensions.netExtensions
{
    public static class ListTExtension
    {
        private static readonly Random _random = new Random();
        public static List<T> Shuffle<T>(this List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = _random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
        }

    }
}
