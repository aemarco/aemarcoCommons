using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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
        var hashBytes = ms.ToHashBytes();
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
        var hashBytes = await ms.ToHashBytesAsync(cancellationToken).ConfigureAwait(false);
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


    /// <summary>
    /// Produces a stable, order-independent Base64 hash of any object.
    /// Collections of <see cref="IComparable"/> elements are sorted before hashing,
    /// so two objects that differ only in list ordering produce the same hash.
    /// Useful as a cache key or equality fingerprint for filter objects.
    /// </summary>
    /// <param name="objectToHash">The object to hash.</param>
    /// <returns>Base64 hash string.</returns>
    public static string ToStableHash(this object objectToHash)
    {
        var json = JsonSerializer.Serialize(objectToHash, CacheKeyOptions);
        return json.ToBase64HashString();
    }

    private static readonly JsonSerializerOptions CacheKeyOptions = new()
    {
        Converters = { new SortedListConverterFactory() }
    };
    private class SortedListConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert) =>
            typeToConvert.IsGenericType &&
            typeToConvert.GetGenericTypeDefinition() == typeof(List<>) &&
            typeToConvert.GetGenericArguments()[0].IsAssignableTo(typeof(IComparable));

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var elementType = typeToConvert.GetGenericArguments()[0];
            return (JsonConverter)Activator.CreateInstance(
                typeof(SortedListConverter<>).MakeGenericType(elementType))!;
        }

        private class SortedListConverter<T> : JsonConverter<List<T>?> where T : IComparable
        {
            public override List<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
                JsonSerializer.Deserialize<List<T>>(ref reader);

            public override void Write(Utf8JsonWriter writer, List<T>? value, JsonSerializerOptions options)
            {
                if (value is null) { writer.WriteNullValue(); return; }
                writer.WriteStartArray();
                foreach (var item in value.Order())
                    JsonSerializer.Serialize(writer, item);
                writer.WriteEndArray();
            }
        }
    }

}