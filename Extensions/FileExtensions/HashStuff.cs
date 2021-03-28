using aemarcoCommons.Extensions.CryptoExtensions;
using System.IO;
using System.Net;

namespace aemarcoCommons.Extensions.FileExtensions
{
    public static class HashStuff
    {
        /// <summary>
        /// reads file or web address content and creates a base64 hash
        /// </summary>
        /// <param name="location">file to be hashes</param>
        /// <returns>base 64 hash string</returns>
        public static string HashTheThing(this string location)
        {
            using (var stream = location.StartsWith("http")
                ? new WebClient().OpenRead(location)
                : File.OpenRead(location))
            {
                return stream.ToBase64HashString();
            }
        }



    }
}
