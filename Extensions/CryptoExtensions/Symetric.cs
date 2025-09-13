using System;
using System.Buffers.Binary;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace aemarcoCommons.Extensions.CryptoExtensions;

public static class Symetric
{

    public static string EncryptToBase64(this string clearText, string password)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(clearText);
        using var memory = new MemoryStream();

        Encrypt(password, memory, cryptoStream =>
        {
            cryptoStream.Write(bytes, 0, bytes.Length);
            cryptoStream.FlushFinalBlock();
        });

        return Convert.ToBase64String(memory.ToArray());
    }
    public static string DecryptFromBase64(this string cryptedBase64, string password)
    {
        var bytesToDecrypt = Convert.FromBase64String(cryptedBase64);
        using var memory = new MemoryStream(bytesToDecrypt);
        byte[] decryptedBytes = null;

        Decrypt(password, memory, cryptoStream =>
        {
            using var ms = new MemoryStream();
            cryptoStream.CopyTo(ms);
            decryptedBytes = ms.ToArray();
        });

        return System.Text.Encoding.UTF8.GetString(decryptedBytes);
    }


    public static async Task<string> EncryptToBase64Async(this string clearText, string password)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(clearText);
        await using var memory = new MemoryStream();

        await EncryptAsync(password, memory, async cryptoStream =>
        {
            await cryptoStream.WriteAsync(bytes);
            await cryptoStream.FlushFinalBlockAsync();
        });

        return Convert.ToBase64String(memory.ToArray());
    }

    public static async Task<string> DecryptFromBase64Async(this string cryptedBase64, string password)
    {
        var bytesToDecrypt = Convert.FromBase64String(cryptedBase64);
        await using var memory = new MemoryStream(bytesToDecrypt);
        byte[] decryptedBytes = null;

        await DecryptAsync(password, memory, async cryptoStream =>
        {
            await using var ms = new MemoryStream();
            await cryptoStream.CopyToAsync(ms);
            decryptedBytes = ms.ToArray();
        });

        return System.Text.Encoding.UTF8.GetString(decryptedBytes);
    }





    public static FileInfo EncryptFile(this FileInfo fileInfo, string password, bool deleteOriginal = false)
    {
        var crypted = new FileInfo($"{fileInfo.FullName}.crypted");
        using (var destinationStream = File.OpenWrite(crypted.FullName))
        {
            Encrypt(
                password,
                destinationStream,
                cryptoStream =>
                {
                    using var readStream = fileInfo.OpenRead();
                    readStream.CopyTo(cryptoStream);
                });
        }
        if (deleteOriginal)
        {
            fileInfo.Delete();
        }
        return crypted;

    }
    public static void EncryptFileInPlace(this FileInfo fileInfo, string password)
    {
        var crypted = fileInfo.EncryptFile(password, true);
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
                    using var destinationStream = decrypted.OpenWrite();
                    cryptoStream.CopyTo(destinationStream);
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



    public static async Task<FileInfo> EncryptFileAsync(this FileInfo fileInfo, string password, bool deleteOriginal = false)
    {
        var crypted = new FileInfo($"{fileInfo.FullName}.crypted");
        await using (var destinationStream = File.OpenWrite(crypted.FullName))
        {
            await EncryptAsync(password, destinationStream, async cryptoStream =>
            {
                await using var readStream = fileInfo.OpenRead();
                await readStream.CopyToAsync(cryptoStream);
            });
        }

        if (deleteOriginal) fileInfo.Delete();
        return crypted;
    }

    public static async Task<FileInfo> DecryptFileAsync(this FileInfo fileInfo, string password, bool deleteCrypted = false)
    {
        var decrypted = new FileInfo($"{fileInfo.FullName}.decrypted");
        if (decrypted.FullName.EndsWith(".crypted.decrypted"))
            decrypted = new FileInfo(decrypted.FullName.Replace(".crypted.decrypted", ".decrypted"));

        await using (var sourceStream = fileInfo.OpenRead())
        {
            await DecryptAsync(password, sourceStream, async cryptoStream =>
            {
                await using var destinationStream = decrypted.OpenWrite();
                await cryptoStream.CopyToAsync(destinationStream);
            });
        }

        if (deleteCrypted) fileInfo.Delete();
        return decrypted;
    }






    #region internal logic

    //this internal logic encrypts stuff symetrical with aes with given pw and keysize
    //the resulting bytes are build up as following: salt + iv + keysize + data

    private static void Encrypt(string passPhrase, Stream destinationStream, Action<CryptoStream> writeAction)
    {
        var settings = EncryptionSettings.Current;

        using var aesAlg = Aes.Create();

        // Write version (Int32)
        Span<byte> versionSpan = stackalloc byte[4];
        BinaryPrimitives.WriteInt32LittleEndian(versionSpan, settings.Version);
        destinationStream.Write(versionSpan);

        // Generate salt using GetRandomBytes
        var salt = GetRandomBytes(aesAlg.BlockSize);
        destinationStream.Write(salt, 0, salt.Length);

        using var pwd = new Rfc2898DeriveBytes(passPhrase, salt, settings.DerivationPasswordIterations, settings.HashAlgorithmName);
        aesAlg.Padding = PaddingMode.PKCS7;

        // Generate IV using GetRandomBytes
        aesAlg.IV = GetRandomBytes(aesAlg.BlockSize);
        destinationStream.Write(aesAlg.IV, 0, aesAlg.IV.Length);

        // Key
        aesAlg.KeySize = settings.KeySize ?? 128;
        aesAlg.Key = pwd.GetBytes(aesAlg.KeySize / 8);

        using var cryptoStream = new CryptoStream(destinationStream, aesAlg.CreateEncryptor(), CryptoStreamMode.Write);
        writeAction(cryptoStream);
    }
    private static void Decrypt(string passPhrase, Stream sourceStream, Action<CryptoStream> readAction)
    {
        Span<byte> versionSpan = stackalloc byte[4];
        sourceStream.ReadExactly(versionSpan);
        int version = BinaryPrimitives.ReadInt32LittleEndian(versionSpan);

        var settings = EncryptionSettings.Create(version);
        if (settings == EncryptionSettings.Default)
            // Old file → no version bytes, reset stream to start
            sourceStream.Position = 0;

        using var aesAlg = Aes.Create();

        // Read salt
        var salt = new byte[aesAlg.BlockSize / 8];
        sourceStream.ReadExactly(salt, 0, salt.Length);

        using var pwd = new Rfc2898DeriveBytes(passPhrase, salt, settings.DerivationPasswordIterations, settings.HashAlgorithmName);
        aesAlg.Padding = PaddingMode.PKCS7;

        // Read IV
        var iv = new byte[aesAlg.BlockSize / 8];
        sourceStream.ReadExactly(iv, 0, iv.Length);

        // Read key size
        aesAlg.KeySize = settings.KeySize ?? 64 * sourceStream.ReadByte(); // Old file → key size byte
        aesAlg.Key = pwd.GetBytes(aesAlg.KeySize / 8);

        using var cryptoStream = new CryptoStream(sourceStream, aesAlg.CreateDecryptor(aesAlg.Key, iv), CryptoStreamMode.Read);
        readAction(cryptoStream);
    }

    private static async Task EncryptAsync(string passPhrase, Stream destinationStream, Func<CryptoStream, Task> writeFunc)
    {
        var settings = EncryptionSettings.Current;
        using var aesAlg = Aes.Create();

        var versionBytes = new byte[4];
        BinaryPrimitives.WriteInt32LittleEndian(versionBytes, settings.Version);
        await destinationStream.WriteAsync(versionBytes);

        var salt = GetRandomBytes(aesAlg.BlockSize);
        await destinationStream.WriteAsync(salt);

        using var pwd = new Rfc2898DeriveBytes(passPhrase, salt, settings.DerivationPasswordIterations, settings.HashAlgorithmName);
        aesAlg.Padding = PaddingMode.PKCS7;

        aesAlg.IV = GetRandomBytes(aesAlg.BlockSize);
        await destinationStream.WriteAsync(aesAlg.IV);

        aesAlg.KeySize = settings.KeySize ?? 128;
        aesAlg.Key = pwd.GetBytes(aesAlg.KeySize / 8);

        await using var cryptoStream = new CryptoStream(destinationStream, aesAlg.CreateEncryptor(), CryptoStreamMode.Write);
        await writeFunc(cryptoStream);
    }

    private static async Task DecryptAsync(string passPhrase, Stream sourceStream, Func<CryptoStream, Task> readFunc)
    {
        var versionBytes = new byte[4];
        await sourceStream.ReadExactlyAsync(versionBytes);
        int version = BinaryPrimitives.ReadInt32LittleEndian(versionBytes);

        var settings = EncryptionSettings.Create(version);
        if (settings == EncryptionSettings.Default)
            sourceStream.Position = 0;

        using var aesAlg = Aes.Create();

        var salt = new byte[aesAlg.BlockSize / 8];
        await sourceStream.ReadExactlyAsync(salt);

        using var pwd = new Rfc2898DeriveBytes(passPhrase, salt, settings.DerivationPasswordIterations, settings.HashAlgorithmName);
        aesAlg.Padding = PaddingMode.PKCS7;

        var iv = new byte[aesAlg.BlockSize / 8];
        await sourceStream.ReadExactlyAsync(iv);

        aesAlg.KeySize = settings.KeySize ?? 64 * sourceStream.ReadByte();
        aesAlg.Key = pwd.GetBytes(aesAlg.KeySize / 8);

        await using var cryptoStream = new CryptoStream(sourceStream, aesAlg.CreateDecryptor(aesAlg.Key, iv), CryptoStreamMode.Read);
        await readFunc(cryptoStream);
    }


    private record EncryptionSettings(int Version, int DerivationPasswordIterations, HashAlgorithmName HashAlgorithmName, int? KeySize)
    {
        public static EncryptionSettings Default => new(0, 10_000, HashAlgorithmName.SHA1, null);
        public static EncryptionSettings Current => new(1, 200_000, HashAlgorithmName.SHA256, 128);
        public static EncryptionSettings Create(int version)
        {
            var result = version switch
            {
                1 => Current,
                _ => Default
            };
            return result;
        }
    }


    /// <summary>
    /// Generates cryptographically secure random bytes.
    /// </summary>
    /// <param name="numberOfBits">Number of bits to generate (must be multiple of 8).</param>
    /// <returns>Random byte array.</returns>
    internal static byte[] GetRandomBytes(int numberOfBits)
    {
        if (numberOfBits % 8 != 0)
            throw new ArgumentException("Number of bits must be a multiple of 8.", nameof(numberOfBits));

        var randomBytes = new byte[numberOfBits / 8];
        RandomNumberGenerator.Fill(randomBytes);
        return randomBytes;
    }



    #endregion

}
