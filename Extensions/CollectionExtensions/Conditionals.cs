using System.Collections.Generic;
using System.Linq;

namespace aemarcoCommons.Extensions.CollectionExtensions
{
    public static class Conditionals
    {
       
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



    }
}
