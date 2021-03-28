using System;
using System.Security.Cryptography;
using System.Text;
// ReSharper disable All

namespace aemarcoCommons.Toolbox.CryptoTools
{
    [Obsolete]
    public static class AesTextCipher
    {
        public static string Encrypt(string input, string iv, string key)
        {
            if (input == null) return null;

            var plainTextBytes = Encoding.ASCII.GetBytes(input);
            using (var aes = new AesCryptoServiceProvider
            {
                BlockSize = 128,
                KeySize = 256,
                Key = Encoding.ASCII.GetBytes(key),
                IV = Encoding.ASCII.GetBytes(iv),
                Padding = PaddingMode.PKCS7,
                Mode = CipherMode.CBC
            })
            {
                using (var crypto = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    var encrypted = crypto.TransformFinalBlock(plainTextBytes, 0, plainTextBytes.Length);
                    return Convert.ToBase64String(encrypted);
                }
            }
        }


        public static string Decrypt(string input, string iv, string key)
        {
            if (input == null) return null;


            var encryptedBytes = Convert.FromBase64String(input);
            using (var aes = new AesCryptoServiceProvider
            {
                BlockSize = 128,
                KeySize = 256,
                Key = Encoding.ASCII.GetBytes(key),
                IV = Encoding.ASCII.GetBytes(iv),
                Padding = PaddingMode.PKCS7,
                Mode = CipherMode.CBC
            })
            {
                using (var crypto = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    var secret = crypto.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                    return Encoding.ASCII.GetString(secret);
                }
            }
        }
    }
}