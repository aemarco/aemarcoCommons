using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace Extensions.netExtensions
{
    public static class StreamExtensions
    {

        /// <summary>
        /// Hash the stream to hashed bytes
        /// </summary>
        /// <param name="stream">stream to hash</param>
        /// <returns>hashed bytes</returns>
        public static IEnumerable<byte> ToHashBytes(this Stream stream)
        {
            using var md5 = MD5.Create();
            return md5.ComputeHash(stream);
        }



        /// <summary>
        /// Hash the stream to base64 string
        /// </summary>
        /// <param name="stream">stream to hash</param>
        /// <returns>base64 string</returns>
        public static string ToBase64HashString(this Stream stream)
        {
            using var md5 = MD5.Create();
            return Convert.ToBase64String(md5.ComputeHash(stream));
        }
    }
}
