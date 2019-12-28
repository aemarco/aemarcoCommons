using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Extensions.netExtensions
{
    public static class StringExtensions
    {
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
