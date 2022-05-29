using System;
using System.IO;
using System.Security.Cryptography;

namespace aemarcoCommons.Extensions.CryptoExtensions
{
    public static class Symetric
    {
        public static string EncryptToBase64(this string clearText, string password, KeySize keySize = KeySize.Normal_128)
        {
            using (var memory = new MemoryStream())
            {
                Encrypt(
                    password, 
                    memory,
                    cryptoStream =>
                    {
                        using (var swEncrypt = new StreamWriter(cryptoStream))
                        {
                            swEncrypt.Write(clearText);
                        }
                    },
                    keySize);
                var result = Convert.ToBase64String(memory.ToArray());
                return result;
            }
        }
        public static string DecryptFromBase64(this string cryptedBase64, string password)
        {
            var bytesToDecrpyt = Convert.FromBase64String(cryptedBase64);
            using (var memory = new MemoryStream(bytesToDecrpyt))
            {
                string result = null;
                Decrypt(
                    password, 
                    memory,
                    cryptoStream =>
                    {
                        using (var srDecrypt = new StreamReader(cryptoStream))
                        {
                            result = srDecrypt.ReadToEnd();
                        }
                    });
                return result;
            }
        }

        public static FileInfo EncryptFile(this FileInfo fileInfo, string password, bool deleteOriginal = false, KeySize keySize = KeySize.Normal_128)
        {
            var crypted = new FileInfo($"{fileInfo.FullName}.crypted");
            using (var destinationStream = File.OpenWrite(crypted.FullName))
            {
                Encrypt(
                    password,
                    destinationStream, 
                    cryptoStream =>
                    {
                        using (var readStream = fileInfo.OpenRead())
                        {
                            readStream.CopyTo(cryptoStream);
                        }
                    },
                    keySize);
            }
            if (deleteOriginal)
            {
                fileInfo.Delete();
            }
            return crypted;
                     
        }
        public static void EncryptFileInPlace(this FileInfo fileInfo, string password, KeySize keySize = KeySize.Normal_128)
        {
            var crypted = fileInfo.EncryptFile(password, true, keySize);
            File.Move(crypted.FullName, fileInfo.FullName);
        }


        public static FileInfo DecryptFile(this FileInfo fileInfo, string password, bool deleteCrypted = false)
        {
            var decrypted = new FileInfo($"{fileInfo.FullName}.decrypted");
            if (decrypted.FullName.EndsWith(".crypted.decrypted"))
            {
                decrypted = new FileInfo(decrypted.FullName.Replace(".crypted.decrypted", ".decrypted"));
            }
            using (var sourceStream = fileInfo.OpenRead())
            {
                Decrypt(
                    password, 
                    sourceStream,
                    cryptoStream =>
                    {
                        using (var destinationStream = decrypted.OpenWrite())
                        {
                            cryptoStream.CopyTo(destinationStream);
                        }
                    });
            }
            if (deleteCrypted)
            {
                fileInfo.Delete();
            }
            return decrypted;
        }
        public static void DecryptFileInPlace(this FileInfo fileInfo, string password)
        {
            var decrypted = fileInfo.DecryptFile(password, true);
            File.Move(decrypted.FullName, fileInfo.FullName);
        }


        #region internal logic

        //this internal logic encrypts stuff symetrical with aes with given pw and keysize
        //the resulting bytes are build up as following: salt + iv + key size + data
        
        private const int DerivationPasswordIterations = 10000;

        private static void Encrypt(string passPhrase, Stream destinationStream, Action<CryptoStream> writeAction, KeySize keySize)
        {
            using (var aesAlg = Aes.Create())
            {
                var salt = GetRandomBytes(aesAlg.BlockSize);
                destinationStream.Write(salt, 0, salt.Length);

                using (var pwd = new Rfc2898DeriveBytes(passPhrase, salt, DerivationPasswordIterations))
                { 
                    aesAlg.Padding = PaddingMode.PKCS7;

                    aesAlg.IV = GetRandomBytes(aesAlg.BlockSize);
                    destinationStream.Write(aesAlg.IV, 0, aesAlg.IV.Length);

                    var ks = 64 * (byte)keySize;
                    destinationStream.WriteByte((byte)keySize);
                    aesAlg.KeySize = ks;
                    aesAlg.Key = pwd.GetBytes(ks / 8);

                    using (var cryptoStream = new CryptoStream(
                            destinationStream, 
                            aesAlg.CreateEncryptor(), 
                            CryptoStreamMode.Write))
                    {
                        writeAction(cryptoStream);
                    }
                }
            }
        }

        private static void Decrypt(string passPhrase, Stream sourceStream, Action<CryptoStream> readAction)
        {
            using (var aesAlg = Aes.Create())
            {
                var salt = new byte[aesAlg.BlockSize / 8];
                sourceStream.Read(salt, 0, salt.Length);

                using (var pwd = new Rfc2898DeriveBytes(passPhrase, salt, DerivationPasswordIterations))
                {
                    aesAlg.Padding = PaddingMode.PKCS7;

                    var iv = new byte[aesAlg.BlockSize / 8];
                    sourceStream.Read(iv, 0, iv.Length);

                    aesAlg.KeySize = 64 * sourceStream.ReadByte();
                    var key = pwd.GetBytes(aesAlg.KeySize / 8);

                    using (var cryptoStream = new CryptoStream(
                            sourceStream,  
                            aesAlg.CreateDecryptor(key, iv),
                            CryptoStreamMode.Read))
                    {
                        readAction(cryptoStream);
                    }
                }
            }
        }

        internal static byte[] GetRandomBytes(int numberOfBits)
        {
            var randomBytes = new byte[numberOfBits / 8];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                // Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
                return randomBytes;
            }
        }

        #endregion

    }

    public enum KeySize : byte
    {
        Normal_128 = 2,
        High_192 = 3,
        Highest_256 = 4
    }
}
