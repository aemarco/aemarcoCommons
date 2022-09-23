using System.ComponentModel;

namespace aemarcoCommons.Extensions.TextExtensions
{
    public static class Parsing
    {

        /// <summary>
        /// TryParse as string extension
        /// </summary>
        /// <typeparam name="T">target struct type</typeparam>
        /// <param name="text">text to parse</param>
        /// <param name="value">out the parsed value</param>
        /// <returns>true if success</returns>
        internal static bool TryParse<T>(this string text, out T? value)
            where T : struct
        {
            var converter = TypeDescriptor.GetConverter(typeof(T));
            try
            {
                value = (T?)converter.ConvertFromString(text);
                return true;
            }
            catch
            {
                value = null;
                return false;
            }
        }



    }
}
