using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Primitives;

namespace Extensions.netExtensions
{
    public static class StringExtensions
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

        public static string ToBase64HashString(this string textToHash)
        {
            using var ms = new MemoryStream(Encoding.UTF8.GetBytes(textToHash));
            var hash = ms.ToHashBytes().ToArray();
            return Convert.ToBase64String(hash);
        }





        [Obsolete("refactor this shit")]
        /// <summary>
        /// Gibt einen MD5 Hash als String zurück
        /// </summary>
        /// <param name="textToHash">string der Gehasht werden soll.</param>
        /// <returns>Hash als string.</returns>
        public static string GetMD5Hash(this string textToHash)
        {
            
            // Use input string to calculate MD5 hash
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(textToHash);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }


    }
}
