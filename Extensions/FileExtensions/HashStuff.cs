using aemarcoCommons.Extensions.CryptoExtensions;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace aemarcoCommons.Extensions.FileExtensions;

public static class HashStuff
{

    private static readonly HttpClient HttpClient = new();

    /// <summary>
    /// Reads file or web address content and creates a base64 hash (SHA256).
    /// </summary>
    /// <param name="location">File path or web address to hash.</param>
    /// <returns>Base64 hash string.</returns>
    public static string HashTheThing(this string location)
    {
        using var stream = location.StartsWith("http", StringComparison.OrdinalIgnoreCase)
            ? HttpClient.GetStreamAsync(location).GetAwaiter().GetResult()
            : File.OpenRead(location);

        return stream.ToBase64HashString();
    }

    public static string HashTheThing(this byte[] bytes)
    {
        using var ms = new MemoryStream(bytes);
        return ms.ToBase64HashString();
    }






    /// <summary>
    /// Async version that reads file or web address content and creates a base64 hash (SHA256).
    /// </summary>
    public static async Task<string> HashTheThingAsync(this string location, CancellationToken cancellationToken = default)
    {
        await using var stream = location.StartsWith("http", StringComparison.OrdinalIgnoreCase)
            ? await HttpClient.GetStreamAsync(location, cancellationToken).ConfigureAwait(false)
            : File.OpenRead(location);

        return await stream.ToBase64HashStringAsync(cancellationToken).ConfigureAwait(false);
    }



}