using System;

// ReSharper disable once CheckNamespace
namespace aemarcoCommons.Extensions.TextExtensions
{
    public static class Manipulation
    {

        /// <summary>
        /// Removes the specified substring from the beginning of the current string.
        /// </summary>
        /// <param name="this">The current string instance.</param>
        /// <param name="startToRemove">The substring to remove from the beginning of the current string.</param>
        /// <returns>
        /// A string that is equivalent to the current string except that it does not begin with the specified substring.
        /// If the current string is null, the method returns null.
        /// If the specified substring is null, the method returns the original string unchanged.
        /// </returns>
        [Obsolete("use aemarcoCommons.Extensions.TrimStart(...) instead")]
        public static string TrimStart(this string @this, string startToRemove)
        {
            if (@this is null)
                return null;
            if (startToRemove is null)
                return @this;

            if (!@this.StartsWith(startToRemove, StringComparison.Ordinal))
                return @this;

            return @this.Substring(startToRemove.Length);
        }

        /// <summary>
        /// Removes given endingToRemove from the end of the string
        /// returns it unchanged, if it´s not ending with given endingToRemove
        /// </summary>
        /// <param name="this">text where to remove some ending</param>
        /// <param name="endingToRemove">ending to remove</param>
        /// <returns>modified string</returns>
        [Obsolete("use aemarcoCommons.Extensions.TrimEnd(...) instead")]
        public static string TrimEnd(this string @this, string endingToRemove)
        {
            if (@this is null)
                return null;
            if (endingToRemove is null)
                return @this;
            if (!@this.EndsWith(endingToRemove, StringComparison.Ordinal))
                return @this;
            //if it´s ending with our endingToRemove, LastIndexOf works right.
            return @this.Substring(0, @this.Length - endingToRemove.Length);
        }

    }
}
