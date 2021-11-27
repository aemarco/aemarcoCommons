using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace aemarcoCommons.Extensions.CryptoExtensions
{
    public static class PasswordExtensions
    {
        //128, 192 or 256 bits
        private const int KeySize = 128;

        // This constant determines the number of iterations for the password bytes generation function.
        private const int DerivationIterations = 1000;


        public static string Encrypt(this string plainText, string passPhrase)
        {
            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                // Salt and IV is randomly generated each time, but is prepended to encrypted cipher text
                // so that the same Salt and IV values can be used when decrypting.  
                var saltStringBytes = GenerateRandomEntropy(aesAlg.BlockSize);
                using (var pwd = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
                { 
                    aesAlg.KeySize = KeySize;
                    aesAlg.Padding = PaddingMode.PKCS7;
                    aesAlg.IV = GenerateRandomEntropy(aesAlg.BlockSize);
                    aesAlg.Key = pwd.GetBytes(aesAlg.KeySize / 8);

                    // Create an encryptor to perform the stream transform.
                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    var resultBytes = saltStringBytes;
                    resultBytes = resultBytes.Concat(aesAlg.IV).ToArray();
                    byte[] encrypted;
                    // Create the streams used for encryption.
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                //Write all data to the stream.
                                swEncrypt.Write(plainText);
                            }
                            encrypted = msEncrypt.ToArray();
                        }
                    }
                    // concat the encrypted bytes from the memory stream.
                    resultBytes = resultBytes.Concat(encrypted).ToArray();
                   
                    var result = Convert.ToBase64String(resultBytes);
                    return result;
                }
            }
        }


        public static string Decrypt(this string cipherText, string passPhrase)
        {
            // Get the complete stream of bytes that represent:
            // [n bytes of Salt] + [n bytes of IV] + [n bytes of CipherText]
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                // Get the salt bytes by extracting the first n bytes from the supplied cipherText bytes.
                var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(aesAlg.BlockSize / 8).ToArray();
                using (var pwd = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
                {
                    aesAlg.KeySize = KeySize;
                    aesAlg.Padding = PaddingMode.PKCS7;
                    aesAlg.IV = cipherTextBytesWithSaltAndIv.Skip(saltStringBytes.Length).Take(aesAlg.BlockSize / 8).ToArray();
                    aesAlg.Key = pwd.GetBytes(aesAlg.KeySize / 8);
                   
                    // Get the actual cipher text bytes by removing the first n bytes from the cipherText string.
                    var cipherTextBytes = cipherTextBytesWithSaltAndIv
                        .Skip(saltStringBytes.Length)
                        .Skip( aesAlg.IV.Length)
                        .Take(cipherTextBytesWithSaltAndIv.Length - saltStringBytes.Length -  aesAlg.IV.Length).ToArray();

                    // Create a decryptor to perform the stream transform.
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    // Create the streams used for decryption.
                    using (MemoryStream msDecrypt = new MemoryStream(cipherTextBytes))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {

                                // Read the decrypted bytes from the decrypting stream
                                // and place them in a string.
                                string plaintext = srDecrypt.ReadToEnd();
                                return plaintext;
                            }
                        }
                    }
                }
            }
        }

        private static byte[] GenerateRandomEntropy(int size)
        {
            var randomBytes = new byte[size / 8];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                // Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
                return randomBytes;
            }
        }
    }
}
