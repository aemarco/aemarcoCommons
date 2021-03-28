using System.IO;
using System.Security.Cryptography;

namespace aemarcoCommons.Extensions.CryptoExtensions
{
    public static class ByteStuff
    {

        /// <summary>
        /// Hash the stream to hashed bytes
        /// </summary>
        /// <param name="stream">stream to hash</param>
        /// <returns>hashed bytes</returns>
        public static byte[] ToHashBytes(this Stream stream)
        {
            using (var md5 = MD5.Create())
            {
                return md5.ComputeHash(stream);
            }

        }




    }
}
