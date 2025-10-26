using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace aemarcoCommons.Extensions.CryptoExtensions;

public static class Base64Stuff
{

    /// <summary>
    /// Hash the string to a Base64 string using <see cref="ByteStuff.ToHashBytes"/>.
    /// </summary>
    /// <param name="textToHash">String to hash.</param>
    /// <returns>Base64 hash string.</returns>
    public static string ToBase64HashString(this string textToHash)
    {
        using var ms = new MemoryStream(Encoding.UTF8.GetBytes(textToHash));
        var hashBytes = ms.ToHashBytes(); // Reuse your existing extension
        return Convert.ToBase64String(hashBytes);
    }

    /// <summary>
    /// Async version: Hash the string to a Base64 string using <see cref="ByteStuff.ToHashBytesAsync"/>.
    /// </summary>
    /// <param name="textToHash">String to hash.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>Base64 hash string.</returns>
    public static async Task<string> ToBase64HashStringAsync(this string textToHash, CancellationToken cancellationToken = default)
    {
        await using var ms = new MemoryStream(Encoding.UTF8.GetBytes(textToHash));
        var hashBytes = await ms.ToHashBytesAsync(cancellationToken).ConfigureAwait(false); // Async reuse
        return Convert.ToBase64String(hashBytes);
    }








    /// <summary>
    /// Hash the stream to a Base64 string using <see cref="ByteStuff.ToHashBytes"/>.
    /// </summary>
    /// <param name="stream">Stream to hash.</param>
    /// <returns>Base64 hash string.</returns>
    public static string ToBase64HashString(this Stream stream)
    {
        var hashBytes = stream.ToHashBytes();
        return Convert.ToBase64String(hashBytes);
    }

    /// <summary>
    /// Async version: Hash the stream to a Base64 string using <see cref="ByteStuff.ToHashBytesAsync"/>.
    /// </summary>
    /// <param name="stream">Stream to hash.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>Base64 hash string.</returns>
    public static async Task<string> ToBase64HashStringAsync(this Stream stream, CancellationToken cancellationToken = default)
    {
        var hashBytes = await stream.ToHashBytesAsync(cancellationToken).ConfigureAwait(false);
        return Convert.ToBase64String(hashBytes);
    }



}