using System;
using System.IO;
using System.Text;

namespace aemarcoCommons.Extensions.CryptoExtensions
{
    public static class Base64Stuff
    {

        /// <summary>
        /// Hash the string to base64 string
        /// </summary>
        /// <param name="textToHash"></param>
        /// <returns>base 64 hash string</returns>
        public static string ToBase64HashString(this string textToHash)
        {
            using var ms = new MemoryStream(Encoding.UTF8.GetBytes(textToHash));
            return ms.ToBase64HashString();
        }


        /// <summary>
        /// Hash the stream to base64 string
        /// </summary>
        /// <param name="stream">stream to hash</param>
        /// <returns>base 64 hash string</returns>
        public static string ToBase64HashString(this Stream stream)
        {
            var hashBytes = stream.ToHashBytes();
            return Convert.ToBase64String(hashBytes);
        }



    }
}
