using System.IO;
using System.Text;

namespace aemarcoCommons.Extensions.CryptoExtensions
{
    public static class HexStuff
    {
        public static string ToHexHashString(this string textToHash)
        {
            using var ms = new MemoryStream(Encoding.UTF8.GetBytes(textToHash));
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
