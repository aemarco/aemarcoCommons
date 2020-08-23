using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Extensions.netExtensions;

namespace Extensions.CryptoExtensions
{
    public static class HexStuff
    {
        public static string ToHexHashString(this string textToHash)
        {
            using var ms = new MemoryStream(Encoding.UTF8.GetBytes(textToHash));
            var hash = ms.ToHashBytes();
            var sb = new StringBuilder();
            foreach (var b in hash)
            {
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString();
        }

        



        [Obsolete("refactor this shit")]
        public static string GetMD5Hash(this string textToHash)
        {
            // Use input string to calculate MD5 hash
            using var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(textToHash);
            var hashBytes = md5.ComputeHash(inputBytes);

            // Convert the byte array to hexadecimal string
            var sb = new StringBuilder();
            foreach (var hashByte in hashBytes)
            {
                sb.Append(hashByte.ToString("X2"));
            }
            return sb.ToString();
        }


    }
}
