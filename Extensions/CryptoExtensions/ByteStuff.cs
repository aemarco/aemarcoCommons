using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace aemarcoCommons.Extensions.CryptoExtensions;

public static class ByteStuff
{

    /// <summary>
    /// Hash the stream to hashed bytes using MD5.
    /// </summary>
    /// <param name="stream">Stream to hash.</param>
    /// <returns>Hashed bytes.</returns>
    public static byte[] ToHashBytes(this Stream stream)
    {
        using var md5 = MD5.Create();
        return md5.ComputeHash(stream);
    }

    /// <summary>
    /// Async version of hashing the stream to hashed bytes using MD5.
    /// </summary>
    /// <param name="stream">Stream to hash.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>Hashed bytes.</returns>
    public static async Task<byte[]> ToHashBytesAsync(this Stream stream, CancellationToken cancellationToken = default)
    {
        using var md5 = MD5.Create();
        return await md5.ComputeHashAsync(stream, cancellationToken).ConfigureAwait(false);
    }




}