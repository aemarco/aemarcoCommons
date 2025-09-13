using System.IO;
using System.Text;

namespace aemarcoCommons.Extensions.CryptoExtensions;

public static class HexStuff
{
    /// <summary>
    /// Hashes the text and x2 it to hex (results in 32 long string)
    /// </summary>
    /// <param name="textToHash">text to hash</param>
    /// <returns>hashed string as hex string</returns>
    public static string ToHexHashString(this string textToHash)
    {
        using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(textToHash)))
        {
            var hash = ms.ToHashBytes();
            var sb = new StringBuilder();
            foreach (var b in hash)
            {
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString();
        }
    }
}