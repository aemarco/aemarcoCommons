using System;

namespace aemarcoCommons.Extensions.TextExtensions
{
    public static class Manipulation
    {
        /// <summary>
        /// Removes given endingToRemove from the end of the string
        /// returns it unchanged, if it´s not ending with given endingToRemove
        /// </summary>
        /// <param name="this">text where to remove some ending</param>
        /// <param name="endingToRemove">ending to remove</param>
        /// <returns>modified string</returns>
        public static string TrimEnd(this string @this, string endingToRemove)
        {
            if (@this is null)
                return null;
            if (!@this.EndsWith(endingToRemove, StringComparison.Ordinal))
                return @this;
            //if it´s ending with our endingToRemove, LastIndexOf works right.

            return @this.Substring(0, @this.LastIndexOf(endingToRemove, StringComparison.Ordinal));
        }
    }
}
